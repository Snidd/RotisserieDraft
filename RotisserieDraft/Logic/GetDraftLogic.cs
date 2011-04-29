using RotisserieDraft.Models;

namespace RotisserieDraft.Logic
{
    public static class GetDraftLogic
    {
        public static IDraftLogic FromDraft(Draft draft)
        {
            return new ModifiedRotisserieDraftLogic();
        }

        public static IDraftLogic FromDraftId(int draftId)
        {
            return new ModifiedRotisserieDraftLogic();
        }

        public static IDraftLogic DefaultDraftLogic()
        {
            return new ModifiedRotisserieDraftLogic();
        }
    }
}