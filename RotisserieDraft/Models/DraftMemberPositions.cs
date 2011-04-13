using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RotisserieDraft.Models
{
	public class DraftMemberPositions
	{
		public DraftMemberPositions()
		{
		}

		public virtual int Id { get; set; }
		public virtual int Position { get; set; }

		public virtual Draft Draft { get; set; }
		public virtual Member Member { get; set; }

	}
}