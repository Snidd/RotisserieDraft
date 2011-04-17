using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Exceptions;
using RotisserieDraft.Domain;
using RotisserieDraft.Models;
using RotisserieDraft.Repositories;

namespace RotisserieDraft.Logic
{
	public class ModifiedRotisserieDraftLogic : IDraftLogic
	{
	    public bool IsDraftAvailable(int draftId)
	    {
            IDraftRepository dr = new DraftRepository();
            var draft = dr.GetById(draftId);
	        return draft != null;
	    }

	    public bool IsMyTurn(int draftId, int memberId)
        {
            IDraftRepository dr = new DraftRepository();
            var draft = dr.GetById(draftId);

			if (draft.CurrentTurn == null)
			{
			    return false;
			}

			return draft.CurrentTurn.Id == memberId;
		}

        public bool PickCard(int draftId, int memberId, int cardId)
		{
			if (!IsMyTurn(draftId, memberId))
			{
			    FuturePickCard(draftId, memberId, cardId);
			    return false;
			}

            IDraftRepository dr = new DraftRepository();
            IMemberRepository mr = new MemberRepository();
            ICardRepository cr = new CardRepository();

            var draft = dr.GetById(draftId);
            var member = mr.GetById(memberId);
            var card = cr.GetById(cardId);

			IPickRepository pickRepository = new PickRepository();
			try
			{
				var pick = pickRepository.PickCard(draft, member, card);
			}
			catch (GenericADOException)
			{
				return false;
			}

		    var picks = pickRepository.GetPicksByDraft(draft);

		    IDraftMemberPositionsRepository dmpr = new DraftMemberPositionsRepository();
		    var nextMemberDraftPosition = dmpr.GetDraftMemberPositionByDraftAndPosition(draft,
		                                                                                GetNextPickPosition(picks.Count, draft.DraftSize));

		    draft.CurrentTurn = nextMemberDraftPosition.Member;

			IDraftRepository draftRepository = new DraftRepository();
			draftRepository.Update(draft);

			TryFuturePick(draftId);

			return true;
		}

        public int GetNextPickPosition(int nrOfPicks = 0, int draftSize = 8)
        {
            int finishedRounds = nrOfPicks / (draftSize * 2);
            int posInRound = nrOfPicks % (draftSize * 2);
            if (posInRound < draftSize)
	        {
                return ((finishedRounds + posInRound) % draftSize) + 1;
	        }
	        else
	        {
                return (finishedRounds + (draftSize * 2 - 1 - posInRound)) % draftSize + 1;
	        }
        }

        private void TryFuturePick(int draftId)
		{
            IDraftRepository dr = new DraftRepository();
			IFuturePickRepository fpr = new FuturePickRepository();

            var draft = dr.GetById(draftId);

			var futurePick = fpr.GetNextFuturePick(draft, draft.CurrentTurn);
			if (futurePick != null)
			{
			    var futurePickCard = futurePick.Card;
                fpr.RemoveFuturePick(futurePick);
                
                PickCard(draftId, draft.CurrentTurn.Id, futurePickCard.Id);
			}
		}

		public int OneStepInDraft(int position, int maxSize, bool clockwise)
		{
			if (clockwise)
			{
				position++;
				if (position > maxSize)
					position = 1;

				return position;
			}
			position--;
			if (position < 1)
				position = maxSize;

			return position;
		}

        public bool FuturePickCard(int draftId, int memberId, int cardId)
		{
			IFuturePickRepository fpr = new FuturePickRepository();
            IDraftRepository dr = new DraftRepository();
            IMemberRepository mr = new MemberRepository();
            ICardRepository cr = new CardRepository();

            var draft = dr.GetById(draftId);
            var member = mr.GetById(memberId);
            var card = cr.GetById(cardId);

			try
			{
				fpr.FuturePickCard(draft, member, card);
			}
			catch (GenericADOException)
			{
				return false;
			}

			return true;
		}

	    public int CurrentPickPosition(int draftId)
	    {
	        IDraftRepository dr = new DraftRepository();
            IDraftMemberPositionsRepository dpr = new DraftMemberPositionsRepository();
	        var draft = dr.GetById(draftId);

	        return dpr.GetDraftPosition(draft, draft.CurrentTurn);
	    }

	    public Draft CreateDraft(string draftName, int ownerId, bool randomPositions, int numberOfPicksPerPlayer, params int[] memberIds)
		{
			var draft = new Draft
			              	{
			              		Name = draftName,
			              		CreatedDate = DateTime.Now,
			              		MaximumPicksPerMember = numberOfPicksPerPlayer,
			              	};
			IDraftRepository dr = new DraftRepository();
			IDraftMemberPositionsRepository dmpr = new DraftMemberPositionsRepository();
            IMemberRepository mr = new MemberRepository();

            draft.Owner = mr.GetById(ownerId);

		    List<int> convertedList = memberIds.ToList();

			draft.StartPosition = 1;
			dr.Add(draft);

			var counter = 0;
            var draftSize = memberIds.Count();

            if (randomPositions)
            {
                var rand = new Random();
                for (var i=1;i<=draftSize;i++)
                {   
                    var randomPosition = rand.Next(0, convertedList.Count - 1);
                    dmpr.AddMemberToDraft(draft, mr.GetById(convertedList[randomPosition]), i);
                    convertedList.Remove(convertedList[randomPosition]);
                }
            }
            else
            {
                foreach (var memberId in memberIds)
                {
                    counter++;
                    dmpr.AddMemberToDraft(draft, mr.GetById(memberId), counter);
                }
            }

		    draft = dr.GetById(draft.Id);
			
			draft.DraftSize = draftSize;
            draft.CurrentTurn = mr.GetById(memberIds[0]);

			dr.Update(draft);

			return draft;
		}

        public int CurrentWheelPosition(int draftId)
		{
            IDraftRepository dr = new DraftRepository();
            var draft = dr.GetById(draftId);

			var currentWheelPosition = draft.StartPosition;
			for (int i = 0; i < draft.DraftSize; i++)
			{
				currentWheelPosition = OneStepInDraft(currentWheelPosition, draft.DraftSize, true);
			}
			return currentWheelPosition;
		}


	}
}