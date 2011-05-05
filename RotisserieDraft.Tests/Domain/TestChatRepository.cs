using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using RotisserieDraft.Domain;
using RotisserieDraft.Models;
using RotisserieDraft.Repositories;

namespace RotisserieDraft.Tests.Domain
{
	[TestClass, DeploymentItem(@".\hibernate.cfg.xml")]
	public class TestChatRepository
	{
		private static ISessionFactory _sessionFactory;
		private static Configuration _configuration;

		private readonly Chat[] _chats = new[]
		                {
		                    new Chat { CreatedDate = new DateTime(2001,01,01), Text = "Hej hopp"},
							new Chat { CreatedDate = new DateTime(2001,01,02), Text = "Nummer två"},
							new Chat { CreatedDate = new DateTime(2001,01,03), Text = "Tre"},
		                };

		private readonly Draft[] _drafts = new[]
		                {
		                    new Draft {Name = "Melon", CreatedDate = new DateTime(2005, 2, 3), Public = true}
		                    ,
		                };

		private readonly Member[] _members = new[]
		        {
		            new Member {Email = "a@a.a", FullName = "Anna Adamsson", UserName = "AnnaA"},
		            new Member {Email = "b@b.b", FullName = "Bertil Bengtsson", UserName = "BertilB"},
		            new Member {Email = "b@c.c", FullName = "Christopher Clemedtsson", UserName = "ChristopherC"},
					new Member {Email = "b@d.d", FullName = "David Danielsson", UserName = "DavidD"},
		        };

		[ClassInitialize]
		public static void TestClassSetup(TestContext context)
		{
			_configuration = new Configuration();
			_configuration.Configure();
			_configuration.AddAssembly(typeof(Draft).Assembly);
			_sessionFactory = _configuration.BuildSessionFactory();
		}

		[TestInitialize]
		public void SetupContext()
		{
			new SchemaExport(_configuration).Execute(false, true, false);

			CreateInitialData();
		}

		public void CreateInitialData()
		{
			using (var session = _sessionFactory.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				foreach (var member in _members)
					session.Save(member);

				foreach (var draft in _drafts)
					session.Save(draft);

				transaction.Commit();
			}


			using (var session = _sessionFactory.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				foreach (var chat in _chats)
				{
					chat.Draft = _drafts[0];
					chat.Member = _members[0];

					session.Save(chat);
				}

				transaction.Commit();
			}
		}

		[TestMethod]
		public void CanAddChat()
		{
			IChatRepository chatRepository = new ChatRepository();
			var chat = new Chat {Draft = _drafts[0], Member = _members[1], Text = "testchattext"};
			chatRepository.Add(chat);

			// use session to try to load the chat
			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<Chat>(chat.Id);

				Assert.IsNotNull(fromDb);
				Assert.AreNotSame(fromDb, chat);
				Assert.AreEqual(chat.Text, fromDb.Text);
			}
		}

		[TestMethod]
		public void CanGetChatList()
		{
			IChatRepository chatRepository = new ChatRepository();
			ICollection<Chat> chats = chatRepository.ListByDraft(_drafts[0]);
			List<Chat> chatlist = chats.ToList();

			Assert.AreEqual(chatlist[0].Text, _chats[2].Text);
			Assert.AreEqual(chatlist[1].Text, _chats[1].Text);
			Assert.AreEqual(chatlist[2].Text, _chats[0].Text);
		}

		[TestMethod]
		public void CanGetUpdatedList()
		{
			IChatRepository chatRepository = new ChatRepository();
			ICollection<Chat> chats = chatRepository.ListByDraft(_drafts[0]);
			List<Chat> chatlist = chats.ToList();

			var chat = new Chat { Draft = _drafts[0], Member = _members[1], Text = "testchattext" };
			chatRepository.Add(chat);

			List<Chat> newChats = chatRepository.ListNewChatsFromDraft(_drafts[0], chatlist[0].Id).ToList();

			Assert.AreEqual(1, newChats.Count);
			Assert.AreEqual(newChats[0].Text, chat.Text);
		}
	}
}
