using System;
using System.Collections.Generic;
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
	public class TestDraftRepository
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
					 new Draft {Name = "Microsoft", CreatedDate = new DateTime(2009,5,21), Public = true},
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

				transaction.Commit();
			}
		}

		[TestMethod]
		public void CanAddNewDraft()
		{
		    var draft = new Draft
		                    {
		                        Name = "TestName",
		                        CreatedDate = DateTime.Now,
		                        MaximumPicksPerMember = 75,
                                Owner = _members[0],
            };

			IDraftRepository repository = new DraftRepository();
			repository.Add(draft);


			// use session to try to load the product
			using (var session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<Draft>(draft.Id);
				// Test that the product was successfully inserted
				
				Assert.IsNotNull(fromDb);
				Assert.AreNotSame(draft, fromDb);
				Assert.AreEqual(draft.Name, fromDb.Name);
				Assert.AreEqual(draft.Public, fromDb.Public);
                Assert.AreEqual(draft.Id, fromDb.Id);
                Assert.AreEqual(draft.Owner.Id, fromDb.Owner.Id);
                Assert.AreEqual(draft.MaximumPicksPerMember, fromDb.MaximumPicksPerMember);

				Assert.AreEqual(draft.CreatedDate.ToString(), fromDb.CreatedDate.ToString());
			}
		}

        [TestMethod]
        public void CanAddNewDraftAndUpdateItWithOwner()
        {
            var draft = new Draft
            {
                Name = "TestName",
                CreatedDate = DateTime.Now,
                MaximumPicksPerMember = 75,
            };

            IDraftRepository repository = new DraftRepository();
            repository.Add(draft);

            draft.Owner = _members[0];

            repository.Update(draft);

            // use session to try to load the product);
            using (var session = _sessionFactory.OpenSession())
            {
                var fromDb = session.Get<Draft>(draft.Id);
                // Test that the product was successfully inserted

                Assert.IsNotNull(fromDb);
                Assert.AreNotSame(draft, fromDb);
                Assert.AreEqual(draft.Name, fromDb.Name);
                Assert.AreEqual(draft.Public, fromDb.Public);
                Assert.AreEqual(draft.Id, fromDb.Id);
                Assert.AreEqual(draft.Owner.Id, fromDb.Owner.Id);
                Assert.AreEqual(draft.MaximumPicksPerMember, fromDb.MaximumPicksPerMember);

                Assert.AreEqual(draft.CreatedDate.ToString(), fromDb.CreatedDate.ToString());
            }
        }

		[TestMethod]
		public void CanUpdateExistingDraft()
		{
			var draft = _drafts[0];
			draft.Name = "Yellow Pear";
			IDraftRepository repository = new DraftRepository();
			repository.Update(draft);

			// use session to try to load the product
			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<Draft>(draft.Id);
				Assert.AreEqual(draft.Name, fromDb.Name);
			}
		}

		[TestMethod]
		public void CanRemoveExistingDraft()
		{
			var draft = _drafts[0];
			IDraftRepository repository = new DraftRepository();
			repository.Remove(draft);

			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<Draft>(draft.Id);
				Assert.IsNull(fromDb);
			}
		}

		[TestMethod]
		public void CanGetListFromName()
		{
			IDraftRepository repository = new DraftRepository();
			ICollection<Draft> appleDrafts = repository.GetByName("Apple");

			Assert.AreEqual(2, appleDrafts.Count);
		}
	}
}
