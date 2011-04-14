using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RotisserieDraft.Domain;
using RotisserieDraft.Models;

namespace RotisserieDraft.Repositories
{
	public class MagicColorsRepository : IMagicColorsRepository
	{
		public void Add(MagicColor magiccolor)
		{
			using (var session = NHibernateHelper.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(magiccolor);
				transaction.Commit();
			}
		}

		public void Update(MagicColor magiccolor)
		{
			using (var session = NHibernateHelper.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Update(magiccolor);
				transaction.Commit();
			}
		}

		public void Remove(MagicColor magiccolor)
		{
			using (var session = NHibernateHelper.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete(magiccolor);
				transaction.Commit();
			}
		}

		public MagicColor GetById(int magicColorId)
		{
			using (var session = NHibernateHelper.OpenSession())
				return session.Get<MagicColor>(magicColorId);
		}
	}
}