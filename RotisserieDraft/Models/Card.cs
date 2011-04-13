using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RotisserieDraft.Models
{
	using System;
	using System.Collections.Generic;

	public partial class Card
	{
		public Card()
		{
		}

		public virtual int Id { get; set; }
		
		public virtual string Name { get; set; }
		public virtual string Type { get; set; }
	}
}