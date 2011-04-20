using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace RotisserieDraft.Logic
{
	public class DraftPrincipal : IPrincipal
	{
		public bool IsInRole(string role)
		{
			throw new NotImplementedException();
		}

		public IIdentity Identity
		{
			get { throw new NotImplementedException(); }
		}
	}
}