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
	public class TestMagicColorsRepository
	{
		private static ISessionFactory _sessionFactory;
		private static Configuration _configuration;

		private readonly MagicColor[] _colors = new[]
		                {
		                    new MagicColor {Name = "Red", ShortName = "R"},
							new MagicColor {Name = "Blue", ShortName = "U"},
							new MagicColor {Name = "Black", ShortName = "B"},
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

				transaction.Commit();
			}
		}

		[TestMethod]
		public void CanAddColor()
		{
			var color = new MagicColor {Name = "Green", ShortName = "G"};
			IMagicColorsRepository repository = new MagicColorsRepository();
			repository.Add(color);

			// use session to try to load the product);
			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<MagicColor>(color.Id);

				// Test that the color was successfully inserted
				Assert.IsNotNull(fromDb);
				Assert.AreNotSame(color, fromDb);
				Assert.AreEqual(color.Name, fromDb.Name);
				Assert.AreEqual(color.ShortName, fromDb.ShortName);
			}
		}

		[TestMethod]
		public void CanUpdateExistingColor()
		{
			var color = _colors[0];
			color.Name = "Black";

			IMagicColorsRepository repository = new MagicColorsRepository();
			repository.Update(color);

			// use session to try to load the product
			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<MagicColor>(color.Id);
				Assert.AreEqual(color.Name, fromDb.Name);
			}
		}

		[TestMethod]
		public void CanRemoveExistingColor()
		{
			var color = _colors[0];
			IMagicColorsRepository repository = new MagicColorsRepository();
			repository.Remove(color);

			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<MagicColor>(color.Id);
				Assert.IsNull(fromDb);
			}
		}

		[TestMethod]
		public void CanGetColorFromId()
		{
			IMagicColorsRepository repository = new MagicColorsRepository();
			var color = repository.GetById(_colors[0].Id);

			Assert.AreEqual(_colors[0].Name, color.Name);
		}
	}
}
