using System.Collections.Generic;
using RotisserieDraft.Models;

namespace RotisserieDraft.DraftLogic
{
	public interface IDraftLogic
	{
		bool IsMyTurn(Draft draft, Member member);
		bool PickCard(Draft draft, Member member, Card card);
		bool FuturePickCard(Draft draft, Member member, Card card);

		Draft CreateDraft(string draftName, bool randomPositions, int numberOfPicksPerPlayer, Member owner, params Member[] members);

		int CurrentWheelPosition(Draft draft);
		List<DraftMemberPositions> GetTablePositions(Draft draft);
		bool IsCardPicked(Draft draft, Card card);
	}
}