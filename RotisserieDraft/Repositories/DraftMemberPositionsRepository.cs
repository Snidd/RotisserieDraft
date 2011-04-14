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
	public class DraftMemberPositionsRepository : IDraftMemberPositionsRepository
	{
		public void AddMemberToDraft(Draft draft, Member member, int position)
		{
			var draftMemberPositions = new DraftMemberPositions
			                                            	{Draft = draft, Member = member, Position = position};

			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Save(draftMemberPositions);
				transaction.Commit();
			}

		}

		public void UpdatePosition(Draft draft, Member member, int position)
		{
			var draftMemberPositions = GetDraftMemberPositionByDraftMember(draft, member);

			draftMemberPositions.Position = position;
			
			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Update(draftMemberPositions);
				transaction.Commit();
			}
		}

		public void RemoveMemberFromDraft(Draft draft, Member member)
		{
			var draftMemberPositions = GetDraftMemberPositionByDraftMember(draft, member);
			
			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete(draftMemberPositions);
				transaction.Commit();
			}
		}

		public DraftMemberPositions GetDraftMemberPositionById(int draftMemberPositionsId)
		{
			using (ISession session = NHibernateHelper.OpenSession())
				return session.Get<DraftMemberPositions>(draftMemberPositionsId);
		}

		public DraftMemberPositions GetDraftMemberPositionByDraftMember(Draft draft, Member member)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var draftMemberPositions = session
					.CreateCriteria(typeof(DraftMemberPositions))
					.Add(Restrictions.Eq("Member", member))
					.Add(Restrictions.Eq("Draft", draft))
					.UniqueResult<DraftMemberPositions>();
				return draftMemberPositions;
			}
		}

		public DraftMemberPositions GetDraftMemberPositionByDraftAndPosition(Draft draft, int draftPosition)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var draftMemberPositions = session
					.CreateCriteria(typeof (DraftMemberPositions))
					.Add(Restrictions.Eq("Draft", draft))
					.Add(Restrictions.Eq("Position", draftPosition))
					.UniqueResult<DraftMemberPositions>();

				return draftMemberPositions;
			}
		}

		public int GetDraftPosition(Draft draft, Member member)
		{
			var draftMemberPositions = GetDraftMemberPositionByDraftMember(draft, member);
			if (draftMemberPositions == null)
				return -1;

			return draftMemberPositions.Position;
		}

		public ICollection<DraftMemberPositions> GetDraftPositionsByMember(Member member)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var draftMemberPositions = session
					.CreateCriteria(typeof(DraftMemberPositions))
					.Add(Restrictions.Eq("Member", member))
					.List<DraftMemberPositions>();
				
				return draftMemberPositions;
			}
		}

		public ICollection<DraftMemberPositions> GetMemberPositionsByDraft(Draft draft)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			{
				var draftMemberPositions = session
					.CreateCriteria(typeof(DraftMemberPositions))
					.Add(Restrictions.Eq("Draft", draft))
					.List<DraftMemberPositions>();

				return draftMemberPositions;
			}

		}
	}
}