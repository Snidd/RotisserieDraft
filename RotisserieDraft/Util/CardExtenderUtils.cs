using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using RotisserieDraft.Models;

namespace RotisserieDraft.Util
{
    public static class CardExtenderUtils
    {
        public static bool IsRed(this Card c)
        {
        	return c.CastingCost.Contains("R");
        }

		public static bool IsBlue(this Card c)
		{
			return c.CastingCost.Contains("U");
		}

		public static bool IsGreen(this Card c)
		{
			return c.CastingCost.Contains("G");
		}

		public static bool IsWhite(this Card c)
		{
			return c.CastingCost.Contains("W");
		}

		public static bool IsBlack(this Card c)
		{
			return c.CastingCost.Contains("B");
		}

		public static bool IsArtifact(this Card c)
		{
			return c.Type.ToLower().Contains("artifact");
		}

		public static bool IsLand(this Card c)
		{
			return c.Type.ToLower().Contains("land");
		}

		public static int GetConvertedManaCost(this Card card)
		{
			int nonNumericChars = 0;
			var chars = new List<char>(card.CastingCost);
			foreach (char c in card.CastingCost)
			{
				if (Char.IsNumber(c)) continue;
				chars.Remove(c);
				nonNumericChars++;
			}
			if (chars.Count == 0) return nonNumericChars;
				
			var parseString = new string(chars.ToArray());

			return Int32.Parse(parseString) + nonNumericChars;
		}
    }
}