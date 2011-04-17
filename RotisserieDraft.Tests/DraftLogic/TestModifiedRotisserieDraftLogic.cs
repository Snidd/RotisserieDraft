using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using RotisserieDraft.Domain;
using RotisserieDraft.Logic;
using RotisserieDraft.Models;
using RotisserieDraft.Repositories;

namespace RotisserieDraft.Tests.DraftLogic
{
    [TestClass, DeploymentItem(@".\hibernate.cfg.xml")]
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
							new Card {CastingCost = "2U", Name = "Thirst for Knowledge", Type = "Instant" },
							new Card {CastingCost = "1RG", Name = "Fires of Yavimaya", Type = "Enchantment" },
							new Card {CastingCost = "BBB", Name = "Necropotence", Type = "Enchantment" },
							new Card {CastingCost = "B", Name = "Dark Ritual", Type = "Instant" },
							new Card {CastingCost = "1B", Name = "Dark Confidant", Type = "Creature - Horror" },
							new Card {CastingCost = "B", Name = "Vendetta", Type = "Instant" },
							new Card {CastingCost = "1B", Name = "Terror", Type = "Instant" },
							new Card {CastingCost = "BB", Name = "Nantuko Shade", Type = "Creature - Horror" },
							new Card {CastingCost = "3B", Name = "Phyrexian Scuta", Type = "Creature - Zombie" },
							new Card {CastingCost = "U", Name = "Brainstorm", Type = "Instant" },
							new Card {CastingCost = "U", Name = "Opt", Type = "Instant" },
							new Card {CastingCost = "U", Name = "Ancestrall Recall", Type = "Instant" },
							new Card {CastingCost = "UU", Name = "Counterspell", Type = "Instant" },
							new Card {CastingCost = "UU", Name = "Mana Drain", Type = "Instant" },
							new Card {CastingCost = "2U", Name = "Compulse Research", Type = "Sorcery" },
							new Card {CastingCost = "3U", Name = "Fact or Fiction", Type = "Instant" },
							new Card {CastingCost = "3U", Name = "Deep Analysis", Type = "Sorcery" },
							new Card {CastingCost = "1U", Name = "Daze", Type = "Instant" },
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
            var draft = draftLogic.CreateDraft("My Testdraft", _members[1].Id, false, 75, _members[0].Id, _members[1].Id,
                                               _members[2].Id, _members[3].Id,
                                               _members[4].Id, _members[5].Id, _members[6].Id, _members[7].Id);

            draftLogic.PickCard(draft.Id, _members[0].Id, _cards[5].Id);
            draftLogic.PickCard(draft.Id, _members[1].Id, _cards[11].Id);

            // use session to try to load the pick)
            using (var session = _sessionFactory.OpenSession())
            {
                var fromDb = session.Get<Draft>(draft.Id);

                // Test that the pick was successfully inserted
                Assert.IsNotNull(fromDb);
                Assert.AreNotSame(draft, fromDb);

                Assert.AreEqual(draft.Owner.Id, fromDb.Owner.Id);
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

        [TestMethod]
		public void CanCreateDraftRandomPositions()
		{
			IDraftLogic draftLogic = new ModifiedRotisserieDraftLogic();
            var draft1 = draftLogic.CreateDraft("My Testdraft", _members[1].Id, true, 75, _members[0].Id, _members[1].Id, _members[2].Id, _members[3].Id,
                              _members[4].Id, _members[5].Id, _members[6].Id, _members[7].Id);

            var draft2 = draftLogic.CreateDraft("My Testdraft 2", _members[2].Id, true, 75, _members[0].Id, _members[1].Id, _members[2].Id, _members[3].Id,
                              _members[4].Id, _members[5].Id, _members[6].Id, _members[7].Id);


            var draft3 = draftLogic.CreateDraft("My Testdraft 3", _members[2].Id, true, 75, _members[0].Id, _members[1].Id, _members[2].Id, _members[3].Id,
                              _members[4].Id, _members[5].Id, _members[6].Id, _members[7].Id);


            var draft4 = draftLogic.CreateDraft("My Testdraft 4", _members[2].Id, true, 75, _members[0].Id, _members[1].Id, _members[2].Id, _members[3].Id,
                              _members[4].Id, _members[5].Id, _members[6].Id, _members[7].Id);


            var draft5 = draftLogic.CreateDraft("My Testdraft 5", _members[2].Id, true, 75, _members[0].Id, _members[1].Id, _members[2].Id, _members[3].Id,
                              _members[4].Id, _members[5].Id, _members[6].Id, _members[7].Id);

            IDraftMemberPositionsRepository dmpr = new DraftMemberPositionsRepository();

			// use session to try to load the pick
			using (var session = _sessionFactory.OpenSession())
			{
				var fromDb1 = session.Get<Draft>(draft1.Id);
                var fromDb2 = session.Get<Draft>(draft2.Id);
                var fromDb3 = session.Get<Draft>(draft3.Id);
                var fromDb4 = session.Get<Draft>(draft4.Id);
                var fromDb5 = session.Get<Draft>(draft5.Id);

				var position1 = dmpr.GetDraftMemberPositionByDraftAndPosition(fromDb1, 1);
                var position2 = dmpr.GetDraftMemberPositionByDraftAndPosition(fromDb2, 1);
                var position3 = dmpr.GetDraftMemberPositionByDraftAndPosition(fromDb3, 1);
                var position4 = dmpr.GetDraftMemberPositionByDraftAndPosition(fromDb4, 1);
                var position5 = dmpr.GetDraftMemberPositionByDraftAndPosition(fromDb5, 1);

                if (position1.Member.Id != position2.Member.Id || position1.Member.Id != position3.Member.Id 
                    || position1.Member.Id != position4.Member.Id || position1.Member.Id != position5.Member.Id)
                {
                    return;
                }

                Assert.Fail("Someone should get different place");
			}
		}

        [TestMethod]
        public void CanDoSimple4PlayerDraft()
        {
            IDraftLogic draftLogic = new ModifiedRotisserieDraftLogic();
            var draft = draftLogic.CreateDraft("My Testdraft", _members[1].Id, false, 75, _members[0].Id, _members[1].Id,
                                               _members[2].Id, _members[3].Id);

            using (var sl = new SystemLogic())
            {
                var wasPicked = draftLogic.PickCard(draft.Id, _members[1].Id, _cards[1].Id);
                Assert.IsFalse(wasPicked, "Not Player Bs turn");

                // Card should end up in _member[1]s FuturePick.
                var futurePicks = sl.GetMyFuturePicks(draft.Id, _members[1].Id);
                Assert.AreEqual(1, futurePicks.Count);

                wasPicked = draftLogic.PickCard(draft.Id, _members[0].Id, _cards[0].Id);
                Assert.IsTrue(wasPicked, "Player A should be able to pick");

                // We should now have two picks total in this draft.
                var picks = sl.GetPickList(draft.Id);
                Assert.AreEqual(2, picks.Count);

                //And _member[1] should have no FuturePicks
                futurePicks = sl.GetMyFuturePicks(draft.Id, _members[1].Id);
                Assert.AreEqual(0, futurePicks.Count);

                Assert.IsTrue(draftLogic.IsMyTurn(draft.Id, _members[2].Id), "Should be Player Cs turn");

                wasPicked = draftLogic.PickCard(draft.Id, _members[2].Id, _cards[2].Id);
                Assert.IsTrue(wasPicked, "It should be Player C");

                wasPicked = draftLogic.PickCard(draft.Id, _members[3].Id, _cards[3].Id);
                Assert.IsTrue(wasPicked, "It should be Player D");

                wasPicked = draftLogic.PickCard(draft.Id, _members[3].Id, _cards[4].Id);
                Assert.IsTrue(wasPicked, "It should be Player D");

                wasPicked = draftLogic.PickCard(draft.Id, _members[2].Id, _cards[5].Id);
                Assert.IsTrue(wasPicked, "It should be Player C");

                wasPicked = draftLogic.PickCard(draft.Id, _members[1].Id, _cards[6].Id);
                Assert.IsTrue(wasPicked, "It should be Player B");

                wasPicked = draftLogic.PickCard(draft.Id, _members[0].Id, _cards[7].Id);
                Assert.IsTrue(wasPicked, "It should be Player A");

                wasPicked = draftLogic.PickCard(draft.Id, _members[1].Id, _cards[8].Id);
                Assert.IsTrue(wasPicked, "It should be Player B");
            }
        }
	}
}
