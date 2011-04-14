using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RotisserieDraft.Models
{
	public partial class MagicColor
	{
		public MagicColor()
		{
		}

		public virtual int Id { get; set; }
		
		public virtual string Name { get; set; }
		public virtual string HexValue { get; set; }
		public virtual string ShortName { get; set; }
	}
}