using System.Collections.Generic;
using RotisserieDraft.Models;

namespace RotisserieDraft.Logic
{
	public interface IDraftLogic
	{
	    bool IsDraftAvailable(int draftId);
        bool IsMyTurn(int draftId, int memberId);

        bool PickCard(int draftId, int memberId, int cardId);
        bool FuturePickCard(int draftId, int memberId, int cardId);
        
        int CurrentWheelPosition(int draftId);
	    int CurrentPickPosition(int draftId);

        Draft CreateDraft(string draftName, int ownerId, bool randomPositions, int numberOfPicksPerPlayer, params int[] memberIds);
	}
}