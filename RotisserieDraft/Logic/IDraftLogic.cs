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
        
	    int CurrentPickPosition(int draftId);
        int GetNextPickPosition(int nrOfPicks, int draftSize);

        Draft CreateDraft(string draftName, int ownerId, int numberOfPicksPerPlayer, bool isPublic);
		bool StartDraft(int draftId, bool randomizeSeats);
		bool AddMemberToDraft(int draftId, int memberId, int draftPosition);
		bool AddMemberToDraft(int draftId, int memberId);

	}
}