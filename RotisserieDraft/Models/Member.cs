using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RotisserieDraft.Models
{
	using System;
	using System.Collections.Generic;

	public partial class Member
	{
		public Member()
		{
			
		}

		public virtual int Id { get; set; }

        public virtual string UserName { get; set; }
		public virtual string FullName { get; set; }
		public virtual string Password { get; set; }
		public virtual string Email { get; set; }

		public virtual IList<DraftMemberPositions> DraftPositions { get; set; }

	}
}