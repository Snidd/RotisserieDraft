using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Exceptions;
using NHibernate.Tool.hbm2ddl;
using RotisserieDraft.Domain;
using RotisserieDraft.Models;
using RotisserieDraft.Repositories;

namespace RotisserieDraft.Tests.Domain
{
	[TestClass]
	public class TestDraftMemberPositionsRepository
	{
		private static ISessionFactory _sessionFactory;
		private static Configuration _configuration;

		private readonly Draft[] _drafts = new[]
                 {
                     new Draft {Name = "Melon", CreatedDate = new DateTime(2005,2,3), Public = true},
                     new Draft {Name = "Pear", CreatedDate = new DateTime(2006,6,7), Public = true},
					 new Draft {Name = "Lemon", CreatedDate = new DateTime(2007,5,10), Public = true},
					 new Draft {Name = "Orange", CreatedDate = new DateTime(2008,4,15), Public = true},
					 new Draft {Name = "Apple", CreatedDate = new DateTime(2009,3,20), Public = true},
					 new Draft {Name = "Apple", CreatedDate = new DateTime(2008,4,20), Public = false},
                 };

		private readonly Member[] _members = new[]
		        {
		            new Member {Email = "a@a.a", FullName = "Anna Adamsson"},
		            new Member {Email = "b@b.b", FullName = "Bertil Bengtsson"},
		            new Member {Email = "b@c.c", FullName = "Christopher Clemedtsson"},
					new Member {Email = "b@d.d", FullName = "David Danielsson"},
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
			using (ISession session = _sessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				foreach (var draft in _drafts)
					session.Save(draft);

				foreach (var member in _members)
					session.Save(member);

				transaction.Commit();

				IDraftMemberPositionsRepository repository = new DraftMemberPositionsRepository();
				
				repository.AddMemberToDraft(_drafts[0], _members[0], 1);
				repository.AddMemberToDraft(_drafts[0], _members[1], 2);
				repository.AddMemberToDraft(_drafts[0], _members[2], 3);
				repository.AddMemberToDraft(_drafts[0], _members[3], 4);
				repository.AddMemberToDraft(_drafts[2], _members[3], 1);

			}
		}

		[TestMethod]
		public void CanAddPlayerToDraft()
		{
			IDraftMemberPositionsRepository repository = new DraftMemberPositionsRepository();

			Draft draft1 = _drafts[1];
			Member member1 = _members[0];

			repository.AddMemberToDraft(draft1, member1, 4);

			int positionFromDb = repository.GetDraftPosition(draft1, member1);

			Assert.AreEqual(4, positionFromDb);
		}

		[TestMethod]
		public void CanAddPlayersToDraft()
		{
			IDraftMemberPositionsRepository repository = new DraftMemberPositionsRepository();

			Draft draft1 = _drafts[1];
			Member member1 = _members[0];
			Member member2 = _members[1];
			Member member3 = _members[2];
			Member member4 = _members[3];

			repository.AddMemberToDraft(draft1, member1, 4);
			repository.AddMemberToDraft(draft1, member2, 3);
			repository.AddMemberToDraft(draft1, member3, 2);
			repository.AddMemberToDraft(draft1, member4, 1);

			int positionFromDb = repository.GetDraftPosition(draft1, member1);
			Assert.AreEqual(4, positionFromDb);

			positionFromDb = repository.GetDraftPosition(draft1, member2);
			Assert.AreEqual(3, positionFromDb);

			positionFromDb = repository.GetDraftPosition(draft1, member3);
			Assert.AreEqual(2, positionFromDb);

			positionFromDb = repository.GetDraftPosition(draft1, member4);
			Assert.AreEqual(1, positionFromDb);

		}
		[TestMethod]
		public void CanUpdatePlayerPosition()
		{
			IDraftMemberPositionsRepository repository = new DraftMemberPositionsRepository();
			repository.UpdatePosition(_drafts[0], _members[1], 5);

			int positionFromDb = repository.GetDraftPosition(_drafts[0], _members[1]);

			Assert.AreEqual(5, positionFromDb);
		}

		[TestMethod]
		public void CanGetPlayerFromPosition()
		{
			IDraftMemberPositionsRepository repository = new DraftMemberPositionsRepository();
			var pos = repository.GetDraftMemberPositionByDraftAndPosition(_drafts[0], 3);
			Assert.AreEqual(_members[2].Id, pos.Id);
		}

		[TestMethod]
		public void CanGetPlayerListFromDraft()
		{
			IDraftMemberPositionsRepository repository = new DraftMemberPositionsRepository();
			ICollection<DraftMemberPositions> draftMemberPositions = repository.GetMemberPositionsByDraft(_drafts[0]);

			Assert.AreEqual(4, draftMemberPositions.Count);
		}

		[TestMethod]
		public void CanGetDraftListFromPlayer()
		{
			IDraftMemberPositionsRepository repository = new DraftMemberPositionsRepository();
			ICollection<DraftMemberPositions> draftMemberPositions = repository.GetDraftPositionsByMember(_members[3]);

			Assert.AreEqual(2, draftMemberPositions.Count);

		}
	}
}
