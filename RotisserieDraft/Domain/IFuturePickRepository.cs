using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RotisserieDraft.Models;

namespace RotisserieDraft.Domain
{
	public interface IFuturePickRepository
	{
		FuturePick FuturePickCard(Draft draft, Member member, Card card);
		void RemoveFuturePick(Draft draft, Member member, Card card);
		void RemoveFuturePick(FuturePick pick);

		FuturePick GetFuturePick(Draft draft, Member member, Card card);
		FuturePick GetNextFuturePick(Draft draft, Member member);
		
		ICollection<FuturePick> GetFuturePicksByDraftAndMember(Draft draft, Member member);
		
	}
}