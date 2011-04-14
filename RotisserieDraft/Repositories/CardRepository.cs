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
	public class CardRepository : ICardRepository
	{
		public void Add(Card card)
		{
			using (var session = NHibernateHelper.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(card);
				transaction.Commit();
			}
		}

		public void Update(Card card)
		{
			using (var session = NHibernateHelper.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Update(card);
				transaction.Commit();
			}
		}

		public void Remove(Card card)
		{
			using (var session = NHibernateHelper.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete(card);
				transaction.Commit();
			}
		}

		public Card GetById(int cardId)
		{
			using (var session = NHibernateHelper.OpenSession())
				return session.Get<Card>(cardId);
		}

		public Card GetByName(string name)
		{
			using (var session = NHibernateHelper.OpenSession())
			{
				var card = session
					.CreateCriteria(typeof(Card))
					.Add(Restrictions.Eq("Name", name))
					.UniqueResult<Card>();
				return card;
			}
		}

		public Card FindCard(string searchtext)
		{
			using (var session = NHibernateHelper.OpenSession())
			{
				var card = session
					.CreateCriteria(typeof(Card))
					.Add(Restrictions.Like("Name", searchtext))
					.UniqueResult<Card>();
				return card;
			}
		}

		public Card FindCard(string searchtext, ICollection<MagicColor> colors)
		{
			throw new NotImplementedException();
		}
	}
}