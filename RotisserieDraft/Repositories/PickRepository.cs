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
	public class PickRepository : IPickRepository
	{
		public Pick PickCard(Draft draft, Member member, Card card)
		{
			var pick = new Pick() { Draft = draft, Member = member, Card = card, CreatedDate = DateTime.Now};

			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Save(pick);
				transaction.Commit();
			}

			return pick;
		}

		public void RemovePick(Draft draft, Member member, Card card)
		{
			var pick = GetPick(draft, member, card);
			RemovePick(pick);
		}

		public void RemovePick(Pick pick)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete(pick);
				transaction.Commit();
			}
		}

		public Pick GetPick(Draft draft, Member member, Card card)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var pick = session
					.CreateCriteria(typeof(Pick))
					.Add(Restrictions.Eq("Member", member))
					.Add(Restrictions.Eq("Draft", draft))
					.Add(Restrictions.Eq("Card", card))
					.UniqueResult<Pick>();
				return pick;
			}
		}

		public ICollection<Pick> GetPicksByDraft(Draft draft)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var picks = session
					.CreateCriteria(typeof(Pick))
					.Add(Restrictions.Eq("Draft", draft))
                    .AddOrder(Order.Asc("Id"))
					.List<Pick>();

				return picks;
			}
		}

		public ICollection<Pick> GetPicksByDraftAndMember(Draft draft, Member member)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var picks = session
					.CreateCriteria(typeof(Pick))
					.Add(Restrictions.Eq("Draft", draft))
					.Add(Restrictions.Eq("Member", member))
					.List<Pick>();

				return picks;
			}
		}

		public ICollection<Pick> GetPicksByCard(Card card)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var picks = session
					.CreateCriteria(typeof(Pick))
					.Add(Restrictions.Eq("Card", card))
					.List<Pick>();

				return picks;
			}
		}

        public Pick GetPickByCardAndDraft(Card card, Draft draft)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var picks = session
					.CreateCriteria(typeof(Pick))
					.Add(Restrictions.Eq("Card", card))
					.Add(Restrictions.Eq("Draft", draft))
					.UniqueResult<Pick>();

				return picks;
			}
		}
	}
}