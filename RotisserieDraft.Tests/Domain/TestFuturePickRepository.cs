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
    [TestClass, DeploymentItem(@".\hibernate.cfg.xml")]
	public class TestFuturePickRepository
	{
		private static ISessionFactory _sessionFactory;
		private static Configuration _configuration;

		private static readonly MagicColor[] _colors = new[]
		                {
		                    new MagicColor {Name = "Red", ShortName = "R"},
							new MagicColor {Name = "Green", ShortName = "G"},
							new MagicColor {Name = "White", ShortName = "W"},
							new MagicColor {Name = "Blue", ShortName = "U"},
							new MagicColor {Name = "Black", ShortName = "B"},
		                };

		private static readonly Card[] _cards = new[]
						{
							new Card {CastingCost = "2U", Name = "Thirst for Knowledge", Type = "Instant" },
							new Card {CastingCost = "1RG", Name = "Fires of Yavimaya", Type = "Enchantment" },
							new Card {CastingCost = "BBB", Name = "Necropotence", Type = "Enchantment" },
						};

		private readonly Draft[] _drafts = new[]
                 {
                     new Draft {Name = "Melon", CreatedDate = new DateTime(2005,2,3), Public = true},
                     new Draft {Name = "Pear", CreatedDate = new DateTime(2006,6,7), Public = true},
					 new Draft {Name = "Lemon", CreatedDate = new DateTime(2007,5,10), Public = true},
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
			using (var session = _sessionFactory.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				foreach (var color in _colors)
					session.Save(color);

				foreach (var card in _cards)
					session.Save(card);

				foreach (var member in _members)
					session.Save(member);

				foreach (var draft in _drafts)
					session.Save(draft);

				transaction.Commit();
			}
		}

		[TestMethod]
		public void CanMakePick()
		{
			IFuturePickRepository repository = new FuturePickRepository();
			var pick = repository.FuturePickCard(_drafts[0], _members[0], _cards[0]);

			// use session to try to load the pick
			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<FuturePick>(pick.Id);

				// Test that the pick was successfully inserted
				Assert.IsNotNull(fromDb);
				Assert.AreNotSame(pick, fromDb);
				Assert.AreEqual(pick.Member.FullName, fromDb.Member.FullName);
				Assert.AreEqual(pick.Draft.Name, fromDb.Draft.Name);
				Assert.AreEqual(pick.Card.Name, fromDb.Card.Name);

				Assert.AreEqual(pick.CreatedDate.ToString(),fromDb.CreatedDate.ToString());
			}

		}

		[TestMethod]
		public void CanMakePicks()
		{
			IFuturePickRepository repository = new FuturePickRepository();
			var pick1 = repository.FuturePickCard(_drafts[0], _members[0], _cards[0]);
			var pick2 = repository.FuturePickCard(_drafts[0], _members[0], _cards[1]);
			var pick3 = repository.FuturePickCard(_drafts[0], _members[0], _cards[2]);

			// use session to try to load the pick
			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<FuturePick>(pick1.Id);

				// Test that the pick was successfully inserted
				Assert.IsNotNull(fromDb);
				Assert.AreNotSame(pick1, fromDb);
				Assert.AreEqual(pick1.Member.FullName, fromDb.Member.FullName);
				Assert.AreEqual(pick1.Draft.Name, fromDb.Draft.Name);
				Assert.AreEqual(pick1.Card.Name, fromDb.Card.Name);

				fromDb = session.Get<FuturePick>(pick2.Id);
				// Test that the pick was successfully inserted
				Assert.IsNotNull(fromDb);
				Assert.AreNotSame(pick2, fromDb);
				Assert.AreEqual(pick2.Member.FullName, fromDb.Member.FullName);
				Assert.AreEqual(pick2.Draft.Name, fromDb.Draft.Name);
				Assert.AreEqual(pick2.Card.Name, fromDb.Card.Name);

				fromDb = session.Get<FuturePick>(pick3.Id);
				// Test that the pick was successfully inserted
				Assert.IsNotNull(fromDb);
				Assert.AreNotSame(pick3, fromDb);
				Assert.AreEqual(pick3.Member.FullName, fromDb.Member.FullName);
				Assert.AreEqual(pick3.Draft.Name, fromDb.Draft.Name);
				Assert.AreEqual(pick3.Card.Name, fromDb.Card.Name);
			}

		}

		[TestMethod]
		public void CanGetPickListByDraftAndMember()
		{
			IFuturePickRepository repository = new FuturePickRepository();

			repository.FuturePickCard(_drafts[0], _members[0], _cards[0]);
			repository.FuturePickCard(_drafts[0], _members[0], _cards[1]);
			repository.FuturePickCard(_drafts[0], _members[0], _cards[2]);

			ICollection<FuturePick> picks = repository.GetFuturePicksByDraftAndMember(_drafts[0], _members[0]);

			Assert.AreEqual(3, picks.Count);
		}

		[TestMethod]
		public void CannotPickSameCardTwiceSameMember()
		{
			IFuturePickRepository repository = new FuturePickRepository();

			repository.FuturePickCard(_drafts[0], _members[0], _cards[0]);
			try
			{
				repository.FuturePickCard(_drafts[0], _members[0], _cards[0]);
			}
			catch (GenericADOException)
			{
				return;
			}

			Assert.Fail("Should not be able to pick same card twice");
		}

		[TestMethod]
		public void CanPickSameCardTwiceDifferentMembers()
		{
			IFuturePickRepository repository = new FuturePickRepository();

			repository.FuturePickCard(_drafts[0], _members[0], _cards[0]);
			try
			{
				repository.FuturePickCard(_drafts[0], _members[1], _cards[0]);
			}
			catch (GenericADOException)
			{
				Assert.Fail("Should be able to add the same card twice with different members.");
			}
		}

	}
}
