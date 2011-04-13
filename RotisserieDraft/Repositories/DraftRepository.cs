using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using NHibernate;
using NHibernate.Criterion;
using RotisserieDraft.Domain;
using RotisserieDraft.Models;

namespace RotisserieDraft.Repositories
{
	public class DraftRepository : IDraftRepository
	{
		public void Add(Draft draft)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Save(draft);
				transaction.Commit();
			}
		}

		public void Update(Draft draft)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Update(draft);
				transaction.Commit();
			}
		}

		public void Remove(Draft draft)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete(draft);
				transaction.Commit();
			}
		}

		public Draft GetById(int draftId)
		{
			using (ISession session = NHibernateHelper.OpenSession())
				return session.Get<Draft>(draftId);
		}

		public ICollection<Draft> GetByName(string name)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var drafts = session
					.CreateCriteria(typeof(Draft))
					.Add(Restrictions.Eq("Name", name))
					.List<Draft>();
				return drafts;
			}
		}

	}
}