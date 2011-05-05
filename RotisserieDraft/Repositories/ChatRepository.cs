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
	public class ChatRepository : IChatRepository
	{
		public void Add(Chat chat)
		{
			chat.CreatedDate = DateTime.Now;

			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Save(chat);
				transaction.Commit();
			}
		}

		public void Remove(Chat chat)
		{
			using (ISession session = NHibernateHelper.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete(chat);
				transaction.Commit();
			}
		}

		public ICollection<Chat> ListByDraft(Draft draft)
		{
			using (var session = NHibernateHelper.OpenSession())
			{
				var chats = session
					.CreateCriteria(typeof (Chat))
					.Add(Restrictions.Eq("Draft", draft))
					.AddOrder(Order.Asc("CreatedDate"))
					.List<Chat>();
				return chats;
			}
		}

		public ICollection<Chat> ListNewChatsFromDraft(Draft draft, int latestChatId)
		{
			using (var session = NHibernateHelper.OpenSession())
			{
				var chats = session
					.CreateCriteria(typeof (Chat))
					.Add(Restrictions.Eq("Draft", draft))
					.Add(Restrictions.Gt("Id", latestChatId))
					.AddOrder(Order.Asc("CreatedDate"))
					.List<Chat>();
				return chats;
			}
		}
	}
}