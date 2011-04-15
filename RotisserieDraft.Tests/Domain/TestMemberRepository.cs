using System;
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
	public class TestMemberRepository
	{
		private static ISessionFactory _sessionFactory;
		private static Configuration _configuration;

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

				foreach (var member in _members)
					session.Save(member);

				transaction.Commit();
			}
		}

		[TestMethod]
		public void CanAddNewMember()
		{
			var member = new Member {Email = "me@me.com", FullName = "Magnus", Password = "asdf"};

			IMemberRepository repository = new MemberRepository();
			repository.Add(member);

			// use session to try to load the product
			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<Member>(member.Id);
				// Test that the product was successfully inserted

				Assert.IsNotNull(fromDb);
				Assert.AreNotSame(member, fromDb);
				Assert.AreEqual(member.FullName, fromDb.FullName);
				Assert.AreEqual(member.Email, fromDb.Email);
				Assert.AreEqual(member.Password, fromDb.Password);
			}
		}

		[TestMethod]
		public void CanUpdateExistingMember()
		{
			var member = _members[0];
			member.FullName = "Karl Adam";
			IMemberRepository repository = new MemberRepository();
			repository.Update(member);

			// use session to try to load the product
			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<Member>(member.Id);
				Assert.AreEqual(member.FullName, fromDb.FullName);
			}
		}

		[TestMethod]
		public void CanRemoveExistingMember()
		{
			var member = _members[0];
			IMemberRepository repository = new MemberRepository();
			repository.Remove(member);

			using (ISession session = _sessionFactory.OpenSession())
			{
				var fromDb = session.Get<Member>(member.Id);
				Assert.IsNull(fromDb);
			}
		}

		[TestMethod]
		public void CanGetMemberFromEmail()
		{
			IMemberRepository repository = new MemberRepository();
			Member member = repository.GetByEmail("b@b.b");

			Assert.AreEqual(member.FullName, _members[1].FullName);
			Assert.AreEqual(member.Email, _members[1].Email);
		}

		[TestMethod]
		public void CannotAddIdenticalEmails()
		{
			var member = new Member { Email = "a@a.a", FullName = "Kalle Ada", Password = "asdf" };

			IMemberRepository repository = new MemberRepository();
			try
			{
				repository.Add(member);
			}
			catch (GenericADOException genericAdoException)
			{
				return;
			}

			Assert.Fail("Should not be able to add two emails of same sort");
		}
	}
}
