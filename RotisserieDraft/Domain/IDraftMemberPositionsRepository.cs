using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RotisserieDraft.Models;

namespace RotisserieDraft.Domain
{
	public interface IDraftMemberPositionsRepository
	{
		void AddMemberToDraft(Draft draft, Member member, int position);
		void UpdatePosition(Draft draft, Member member, int position);
		void RemoveMemberFromDraft(Draft draft, Member member);
		
		DraftMemberPositions GetDraftMemberPositionById(int draftMemberPositionsId);
		DraftMemberPositions GetDraftMemberPositionByDraftMember(Draft draft, Member member);
		int GetDraftPosition(Draft draft, Member member);

		ICollection<DraftMemberPositions> GetDraftPositionsByMember(Member member);
		ICollection<DraftMemberPositions> GetMemberPositionsByDraft(Draft draft);

	}
}