using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RotisserieDraft.Models
{
	using System;
	using System.Collections.Generic;

	public partial class Draft
	{
		public Draft()
		{
		}

		public virtual int Id { get; set; }
		public virtual DateTime CreatedDate { get; set; }
		public virtual string Name { get; set; }
		public virtual bool Public { get; set; }
		public virtual int MaximumPicksPerMember { get; set; }
		public virtual int DraftSize { get; set; }
		public virtual Member Owner { get; set; }
		public virtual Member CurrentTurn { get; set; }
		public virtual bool Clockwise { get; set; }
		public virtual int StartPosition { get; set; }

		public virtual IList<DraftMemberPositions> MemberPositions { get; set; }
	}
}