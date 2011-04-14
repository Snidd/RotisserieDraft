using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RotisserieDraft.Models
{
	public partial class FuturePick
	{
		public FuturePick()
		{
		}

		public virtual int Id { get; set; }

		public virtual DateTime CreatedDate { get; set; }
		public virtual Card Card { get; set; }
		public virtual Draft Draft { get; set; }
		public virtual Member Member { get; set; }
	}
}