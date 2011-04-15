using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using RotisserieDraft.Models;

namespace RotisserieDraft.Tests.Tests
{
	[TestClass, DeploymentItem(@".\hibernate.cfg.xml")]
	public class GenerateSchemaTests
	{
		[TestMethod]
		public void CanGenerateSchema()
		{
			var cfg = new Configuration();
			cfg.Configure();
			cfg.AddAssembly(typeof(Draft).Assembly);

			new SchemaExport(cfg).Execute(true, true, false);
		}
	}
}
