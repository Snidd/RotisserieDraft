using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using RotisserieDraft.Domain;
using RotisserieDraft.DraftLogic;
using RotisserieDraft.Models;
using RotisserieDraft.Repositories;

namespace RotisserieDraft.Tests.DraftLogic
{
	[TestClass]
	public class TestModifiedRotisserieDraftLogic
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
							new MagicColor {Name = "Artifact", ShortName = "A"},
							new MagicColor {Name = "Colorless", ShortName = "C"},
		                };

		private static readonly Card[] _cards = new[]
						{
							new Card(_colors[3]) {CastingCost = "2U", Name = "Thirst for Knowledge", Type = "Instant" },
							new Card(_colors[0], _colors[1]) {CastingCost = "1RG", Name = "Fires of Yavimaya", Type = "Enchantment" },
							new Card(_colors[4]) {CastingCost = "BBB", Name = "Necropotence", Type = "Enchantment" },
							new Card(_colors[4]) {CastingCost = "B", Name = "Dark Ritual", Type = "Instant" },
							new Card(_colors[4]) {CastingCost = "1B", Name = "Dark Confidant", Type = "Creature - Horror" },
							new Card(_colors[4]) {CastingCost = "B", Name = "Vendetta", Type = "Instant" },
							new Card(_colors[4]) {CastingCost = "1B", Name = "Terror", Type = "Instant" },
							new Card(_colors[4]) {CastingCost = "BB", Name = "Nantuko Shade", Type = "Creature - Horror" },
							new Card(_colors[4]) {CastingCost = "3B", Name = "Phyrexian Scuta", Type = "Creature - Zombie" },
							new Card(_colors[3]) {CastingCost = "U", Name = "Brainstorm", Type = "Instant" },
							new Card(_colors[3]) {CastingCost = "U", Name = "Opt", Type = "Instant" },
							new Card(_colors[3]) {CastingCost = "U", Name = "Ancestrall Recall", Type = "Instant" },
							new Card(_colors[3]) {CastingCost = "UU", Name = "Counterspell", Type = "Instant" },
							new Card(_colors[3]) {CastingCost = "UU", Name = "Mana Drain", Type = "Instant" },
							new Card(_colors[3]) {CastingCost = "2U", Name = "Compulse Research", Type = "Sorcery" },
							new Card(_colors[3]) {CastingCost = "3U", Name = "Fact or Fiction", Type = "Instant" },
							new Card(_colors[3]) {CastingCost = "3U", Name = "Deep Analysis", Type = "Sorcery" },
							new Card(_colors[3]) {CastingCost = "1U", Name = "Daze", Type = "Instant" },
						};

		private readonly Member[] _members = new[]
		        {
		            new Member {Email = "a@a.a", FullName = "Anna Adamsson"},
		            new Member {Email = "b@b.b", FullName = "Bertil Bengtsson"},
		            new Member {Email = "b@c.c", FullName = "Christopher Clemedtsson"},
					new Member {Email = "d@d.d", FullName = "David Danielsson"},
					new Member {Email = "e@e.e", FullName = "Erik Eggbert"},
					new Member {Email = "f@f.f", FullName = "Fredrik Fitipaldi"},
					new Member {Email = "g@g.g", FullName = "Gustav Grönlund"},
					new Member {Email = "h@h.h", FullName = "Harald Hansson"},
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

				transaction.Commit();
			}
		}

		[TestMethod]
		public void CanCreateDraftFixedPositions()
		{
			IDraftLogic draftLogic = new ModifiedRotisserieDraftLogic();
			var draft = draftLogic.CreateDraft("My Testdraft", false, 75, _members[1], _members[0], _members[1], _members[2], _members[3],
			                  _members[4], _members[5], _members[6], _members[7]);


			// use session to try to load the pick
			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<Draft>(draft.Id);

				// Test that the pick was successfully inserted
				Assert.IsNotNull(fromDb);
				Assert.AreNotSame(draft, fromDb);
				Assert.AreEqual(draft.Owner.FullName, fromDb.Owner.FullName);
				Assert.AreEqual(draft.DraftSize, fromDb.DraftSize);
				Assert.AreEqual(draft.MaximumPicksPerMember, fromDb.MaximumPicksPerMember);
				Assert.AreEqual(8, draft.DraftSize);
				Assert.AreEqual(_members[0].Id, draft.CurrentTurn.Id);
			
				IDraftMemberPositionsRepository dmpr = new DraftMemberPositionsRepository();

				ICollection<DraftMemberPositions> testList = dmpr.GetMemberPositionsByDraft(draft);
				List<DraftMemberPositions> test = testList.ToList();

				var position = dmpr.GetDraftMemberPositionByDraftAndPosition(fromDb, 4);
				Assert.AreEqual(_members[3].Id, position.Id);

				position = dmpr.GetDraftMemberPositionByDraftAndPosition(fromDb, 8);
				Assert.AreEqual(_members[7].Id, position.Id);
				position = dmpr.GetDraftMemberPositionByDraftAndPosition(fromDb, 7);
				Assert.AreEqual(_members[6].Id, position.Id);
				position = dmpr.GetDraftMemberPositionByDraftAndPosition(fromDb, 6);
				Assert.AreEqual(_members[5].Id, position.Id);
				position = dmpr.GetDraftMemberPositionByDraftAndPosition(fromDb, 5);
				Assert.AreEqual(_members[4].Id, position.Id);
				position = dmpr.GetDraftMemberPositionByDraftAndPosition(fromDb, 3);
				Assert.AreEqual(_members[2].Id, position.Id);
				position = dmpr.GetDraftMemberPositionByDraftAndPosition(fromDb, 2);
				Assert.AreEqual(_members[1].Id, position.Id);
				position = dmpr.GetDraftMemberPositionByDraftAndPosition(fromDb, 1);
				Assert.AreEqual(_members[0].Id, position.Id);

			}
		}
	}
}
