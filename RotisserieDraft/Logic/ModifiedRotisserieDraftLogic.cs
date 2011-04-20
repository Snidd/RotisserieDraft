using System;
using System.Collections;
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

			if (draft.Finished || !draft.Started)
			{
				return false;
			}

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

		public bool StartDraft(int draftId, bool randomizeSeats)
		{
			IDraftRepository dr = new DraftRepository();
			IDraftMemberPositionsRepository dmpr = new DraftMemberPositionsRepository();

			var draft = dr.GetById(draftId);

			if (randomizeSeats)
			{
				var draftMembers = dmpr.GetMemberPositionsByDraft(draft).ToList();
				var rand = new Random();

				for (var i = 1; i <= draft.DraftSize; i++)
				{
					var randomPlayer = rand.Next(0, draftMembers.Count - 1);
					var member = draftMembers[randomPlayer].Member;
					dmpr.UpdatePosition(draft, member, i);
					draftMembers.RemoveAt(randomPlayer);
				}
			}

			var startingPlayer = dmpr.GetDraftMemberPositionByDraftAndPosition(draft, 1);

			draft.Started = true;
			draft.CurrentTurn = startingPlayer.Member;

			dr.Update(draft);

			return false;
		}

		public bool AddMemberToDraft(int draftId, int memberId)
		{
			IDraftMemberPositionsRepository dmpr = new DraftMemberPositionsRepository();
			IDraftRepository dr = new DraftRepository();
			IMemberRepository mr = new MemberRepository();

			var draft = dr.GetById(draftId);

			ICollection<DraftMemberPositions> draftMembers = dmpr.GetMemberPositionsByDraft(draft);

			int newPosition = 0;
			bool foundPosition = true;

			while (foundPosition)
			{
				foundPosition = false;
				newPosition++;

				foreach (var draftMember in draftMembers)
				{
					if (draftMember.Position == newPosition)
					{
						foundPosition = true;
					}
				}
				
			}
			try
			{
				dmpr.AddMemberToDraft(draft, mr.GetById(memberId), newPosition);
				draft.DraftSize++;
				dr.Update(draft);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool AddMemberToDraft(int draftId, int memberId, int draftPosition)
		{
			IDraftMemberPositionsRepository dmpr = new DraftMemberPositionsRepository();
			IDraftRepository dr = new DraftRepository();
			IMemberRepository mr = new MemberRepository();

			try
			{
				var draft = dr.GetById(draftId);
				dmpr.AddMemberToDraft(draft, mr.GetById(memberId), draftPosition);
				draft.DraftSize++;
				dr.Update(draft);
				return true;
			}
			catch
			{
				return false;
			}
		}

	    public Draft CreateDraft(string draftName, int ownerId, int numberOfPicksPerPlayer, bool isPublic)
	    {
	    	var draft = new Draft
	    	            	{
	    	            		Name = draftName,
	    	            		CreatedDate = DateTime.Now,
	    	            		MaximumPicksPerMember = numberOfPicksPerPlayer,
	    	            		Public = isPublic,
								Started = false,
								Finished = false,
			              	};

			IDraftRepository dr = new DraftRepository();
			IDraftMemberPositionsRepository dmpr = new DraftMemberPositionsRepository();
            IMemberRepository mr = new MemberRepository();

            draft.Owner = mr.GetById(ownerId);
	    	draft.DraftSize = 0;

			dr.Add(draft);


			return draft;
		}
	}
}