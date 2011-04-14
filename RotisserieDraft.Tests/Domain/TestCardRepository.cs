using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using RotisserieDraft.Domain;
using RotisserieDraft.Models;
using RotisserieDraft.Repositories;

namespace RotisserieDraft.Tests.Domain
{
	[TestClass]
	public class TestCardRepository
	{
		private static ISessionFactory _sessionFactory;
		private static Configuration _configuration;

		private readonly MagicColor[] _colors = new[]
		                {
		                    new MagicColor {Name = "Red", ShortName = "R"},
							new MagicColor {Name = "Green", ShortName = "G"},
							new MagicColor {Name = "White", ShortName = "W"},
							new MagicColor {Name = "Blue", ShortName = "U"},
							new MagicColor {Name = "Black", ShortName = "B"},
		                };

		// The _cards initial data is filled in the CreateInitialData method.
		private Card[] _cards;

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

				transaction.Commit();
			}

			_cards = new[]
						{
							new Card(_colors[3]) {CastingCost = "2U", Name = "Thirst for Knowledge", Type = "Instant" },
							new Card(_colors[0], _colors[1]) {CastingCost = "1RG", Name = "Fires of Yavimaya", Type = "Enchantment" },
						};

			using (var session = _sessionFactory.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				foreach (var card in _cards)
					session.Save(card);

				transaction.Commit();
			}
		}

		[TestMethod]
		public void CanAddCard()
		{
			var card = new Card(_colors[2]) {Name = "White Knight", CastingCost = "WW", Type = "Creature - Knight"};

			ICardRepository repository = new CardRepository();
			repository.Add(card);

			// use session to try to load the product);
			using (var session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<Card>(card.Id);

				// Test that the color was successfully inserted
				Assert.IsNotNull(fromDb);
				Assert.AreNotSame(card, fromDb);
				Assert.AreEqual(card.Name, fromDb.Name);
				Assert.AreEqual(card.CastingCost, fromDb.CastingCost);
				Assert.AreEqual(card.Type, fromDb.Type);

				Assert.IsNotNull(fromDb.Colors);
				Assert.AreEqual(1, fromDb.Colors.Count);

				Assert.AreEqual(_colors[2].Name, fromDb.Colors[0].Name);
			}
		}

		[TestMethod]
		public void CanUpdateExistingCard()
		{
			var card = _cards[0];
			card.Name = "Törst efter Kunskap";

			ICardRepository repository = new CardRepository();
			repository.Update(card);

			// use session to try to load the product
			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<Card>(card.Id);
				Assert.AreEqual(card.Name, fromDb.Name);
			}
		}

		[TestMethod]
		public void CanRemoveExistingCard()
		{
			var card = _cards[0];
			ICardRepository repository = new CardRepository();
			repository.Remove(card);

			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<Card>(card.Id);
				Assert.IsNull(fromDb);
			}
		}
	}
}
