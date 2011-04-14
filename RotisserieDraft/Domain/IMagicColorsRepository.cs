using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RotisserieDraft.Models;

namespace RotisserieDraft.Domain
{
	public interface IMagicColorsRepository
	{
		void Add(MagicColor magiccolor);
		void Update(MagicColor magiccolor);
		void Remove(MagicColor magiccolor);
		MagicColor GetById(int magicColorId);
	}
}