using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Criterion;
using RotisserieDraft.Domain;
using RotisserieDraft.Models;

namespace RotisserieDraft.Repositories
{
	public class FuturePickRepository : IFuturePickRepository
	{
		public FuturePick FuturePickCard(Draft draft, Member member, Card card)
		{
			var pick = new FuturePick() { Draft = draft, Member = member, Card = card, CreatedDate = DateTime.Now };

			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Save(pick);
				transaction.Commit();
			}

			return pick;
		}

		public FuturePick GetNextFuturePick(Draft draft, Member member)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var pick = session
					.CreateCriteria(typeof (FuturePick))
					.Add(Restrictions.Eq("Member", member))
					.Add(Restrictions.Eq("Draft", draft))
					.AddOrder(Order.Asc("CreatedDate"))
					.List<FuturePick>();
				
				if (pick.Count > 0)
					return pick[0];
	
				return null;
			}
		}

		public void RemoveFuturePick(Draft draft, Member member, Card card)
		{
			var pick = GetFuturePick(draft, member, card);
			RemoveFuturePick(pick);
		}

		public void RemoveFuturePick(FuturePick pick)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete(pick);
				transaction.Commit();
			}
		}

		public FuturePick GetFuturePick(Draft draft, Member member, Card card)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var pick = session
					.CreateCriteria(typeof(FuturePick))
					.Add(Restrictions.Eq("Member", member))
					.Add(Restrictions.Eq("Draft", draft))
					.Add(Restrictions.Eq("Card", card))
					.UniqueResult<FuturePick>();
				return pick;
			}
		}

		public ICollection<FuturePick> GetFuturePicksByDraftAndMember(Draft draft, Member member)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var picks = session
					.CreateCriteria(typeof(FuturePick))
					.Add(Restrictions.Eq("Draft", draft))
					.Add(Restrictions.Eq("Member", member))
                    .AddOrder(Order.Desc("CreatedDate"))
					.List<FuturePick>();

				return picks;
			}
		}

	}
}