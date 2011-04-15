using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RotisserieDraft.Models;

namespace RotisserieDraft.Util
{
    public static class CardExtenderUtils
    {
        public static List<MagicColor> GetColors(this Card c)
        {
            return new List<MagicColor>();
        }
    }
}