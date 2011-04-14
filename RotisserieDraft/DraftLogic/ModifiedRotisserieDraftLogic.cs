using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Exceptions;
using RotisserieDraft.Domain;
using RotisserieDraft.Models;
using RotisserieDraft.Repositories;

namespace RotisserieDraft.DraftLogic
{
	public class ModifiedRotisserieDraftLogic : IDraftLogic
	{
		public bool IsMyTurn(Draft draft, Member member)
		{
			if (draft.CurrentTurn == null)
			{
				IDraftMemberPositionsRepository draftMemberPositionsRepository = new DraftMemberPositionsRepository();
				var position = draftMemberPositionsRepository.GetDraftPosition(draft, member);
				return position == 1;
			}

			return draft.CurrentTurn.Id == member.Id;
		}

		public bool PickCard(Draft draft, Member member, Card card)
		{
			if (!IsMyTurn(draft, member))
				return false;

			IPickRepository pickRepository = new PickRepository();
			try
			{
				var pick = pickRepository.PickCard(draft, member, card);
			}
			catch (GenericADOException)
			{
				return false;
			}

			IDraftMemberPositionsRepository dmpr = new DraftMemberPositionsRepository();
			var draftMemberPosition = dmpr.GetDraftMemberPositionByDraftMember(draft, member);

			if (draft.Clockwise)
			{
				var noPicks = NumberOfPicksSinceLastStart(draft.StartPosition, draftMemberPosition.Position, draft.DraftSize, draft.Clockwise);
				if (noPicks == draft.DraftSize)
				{
					draft.Clockwise = false;
				}
				else
				{
					var nextPosition = OneStepInDraft(draftMemberPosition.Position, draft.DraftSize, draft.Clockwise);
					draftMemberPosition = dmpr.GetDraftMemberPositionByDraftAndPosition(draft, nextPosition);
					draft.CurrentTurn = draftMemberPosition.Member;
				}
			}
			else
			{
				var noPicks = NumberOfPicksSinceLastStart(draft.StartPosition, draftMemberPosition.Position, draft.DraftSize, draft.Clockwise);
				if (noPicks == draft.DraftSize*2)
				{
					draft.Clockwise = true;
					var nextPosition = OneStepInDraft(draft.StartPosition, draft.DraftSize, true);
					draftMemberPosition = dmpr.GetDraftMemberPositionByDraftAndPosition(draft, nextPosition);
					draft.CurrentTurn = draftMemberPosition.Member;
					draft.StartPosition = nextPosition;
				}
				else
				{
					var nextPosition = OneStepInDraft(draft.StartPosition, draft.DraftSize, draft.Clockwise);
					draftMemberPosition = dmpr.GetDraftMemberPositionByDraftAndPosition(draft, nextPosition);
					draft.CurrentTurn = draftMemberPosition.Member;
				}
			}

			IDraftRepository draftRepository = new DraftRepository();
			draftRepository.Update(draft);

			TryFuturePick(draft);

			return true;
		}

		private void TryFuturePick(Draft draft)
		{
			IFuturePickRepository fpr = new FuturePickRepository();
			var futurePick = fpr.GetNextFuturePick(draft, draft.CurrentTurn);
			if (futurePick != null)
			{
				var cardPicked = PickCard(draft, draft.CurrentTurn, futurePick.Card);
			}
		}

		private int OneStepInDraft(int position, int maxSize, bool clockwise)
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

		private int NumberOfPicksSinceLastStart(int startPosition, int currentPosition, int maxSize, bool clockwise)
		{
			var counter = 0;

			if (clockwise)
			{
				while (startPosition != currentPosition)
				{
					startPosition = OneStepInDraft(startPosition, maxSize, true);
					counter++;
				}
			}
			else
			{
				while (startPosition != currentPosition)
				{
					startPosition = OneStepInDraft(startPosition, maxSize, false);
					counter++;
				}
			}

			return counter;
		}

		public bool FuturePickCard(Draft draft, Member member, Card card)
		{
			IFuturePickRepository fpr = new FuturePickRepository();
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

		public Draft CreateDraft(string draftName, bool randomPositions, int numberOfPicksPerPlayer, Member owner, params Member[] members)
		{
			var draft = new Draft
			              	{
			              		Name = draftName,
			              		CreatedDate = DateTime.Now,
			              		MaximumPicksPerMember = numberOfPicksPerPlayer,
			              		Owner = owner
			              	};
			IDraftRepository dr = new DraftRepository();
			IDraftMemberPositionsRepository dmpr = new DraftMemberPositionsRepository();

			draft.StartPosition = 1;
			dr.Add(draft);

			var counter = 0;
			foreach (var member in members)
			{
				counter++;
				dmpr.AddMemberToDraft(draft, member, counter);
			}


			
			

			


			
			draft.DraftSize = counter;
			draft.CurrentTurn = members[0];

			dr.Update(draft);

			return draft;
		}

		public int CurrentWheelPosition(Draft draft)
		{
			var currentWheelPosition = draft.StartPosition;
			for (int i = 0; i < draft.DraftSize; i++)
			{
				currentWheelPosition = OneStepInDraft(currentWheelPosition, draft.DraftSize, true);
			}
			return currentWheelPosition;
		}

		public List<DraftMemberPositions> GetTablePositions(Draft draft)
		{
			return draft.MemberPositions.ToList();
		}

		public bool IsCardPicked(Draft draft, Card card)
		{
			IPickRepository pr = new PickRepository();
			ICollection<Pick> picks = pr.GetPicksByCardAndDraft(card, draft);
			return picks.Count > 0;
		}
	}
}