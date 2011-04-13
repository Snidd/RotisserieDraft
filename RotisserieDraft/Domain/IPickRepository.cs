﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RotisserieDraft.Models;

namespace RotisserieDraft.Domain
{
	public interface IPickRepository
	{
		void PickCard(Draft draft, Member member, Card card);
		void RemovePick(Draft draft, Member member, Card card);
		void RemovePick(int pickId);

		ICollection<Pick> GetPicksByDraft(Draft draft);
		ICollection<Pick> GetPicksByDraftAndMember(Draft draft, Member member);

		ICollection<Pick> GetPicksByCard(Card card);
		ICollection<Pick> GetPicksByCardAndDraft(Card card, Draft draft);
	}
}