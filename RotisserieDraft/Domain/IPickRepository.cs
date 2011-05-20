using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RotisserieDraft.Models;

namespace RotisserieDraft.Domain
{
	public interface IPickRepository
	{
		Pick PickCard(Draft draft, Member member, Card card);
		void RemovePick(Draft draft, Member member, Card card);
		void RemovePick(Pick pick);

		Pick GetPick(Draft draft, Member member, Card card);

		ICollection<Pick> GetPicksByDraft(Draft draft);
		ICollection<Pick> GetPicksByDraftAndMember(Draft draft, Member member);

		ICollection<Pick> GetPicksByCard(Card card);
		Pick GetPickByCardAndDraft(Card card, Draft draft);
	}
}