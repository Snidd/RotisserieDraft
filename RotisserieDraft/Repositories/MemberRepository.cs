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
	public class MemberRepository : IMemberRepository
	{
		public void Add(Member member)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Save(member);
				transaction.Commit();
			}
		}

		public void Update(Member member)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Update(member);
				transaction.Commit();
			}
		}

		public void Remove(Member member)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete(member);
				transaction.Commit();
			}
		}

		public Member GetById(int memberId)
		{
			using (ISession session = NHibernateHelper.OpenSession())
				return session.Get<Member>(memberId);
		}

		public Member GetByEmail(string email)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var member = session
					.CreateCriteria(typeof(Member))
					.Add(Restrictions.Eq("Email", email))
					.UniqueResult<Member>();
				return member;
			}
		}

	    public Member GetByUsername(string username)
	    {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                var member = session
                    .CreateCriteria(typeof(Member))
                    .Add(Restrictions.Eq("UserName", username))
                    .UniqueResult<Member>();
                return member;
            }
	    }
	}
}