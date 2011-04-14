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

		public Card(params MagicColor[] colors)
		{
			foreach (var color in colors)
			{
				this.Colors = new List<MagicColor>();
				this.Colors.Add(color);
			}
		}

		public virtual int Id { get; set; }
		
		public virtual string Name { get; set; }
		public virtual string Type { get; set; }
		public virtual string CastingCost { get; set; }
		public virtual string PowerToughness { get; set; }
		public virtual string Text { get; set; }

		public virtual IList<MagicColor> Colors { get; set; }
	}
}