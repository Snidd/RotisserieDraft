using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RotisserieDraft.Models;

namespace RotisserieDraft.Domain
{
	public interface IMagicColorsRepository
	{
		void Add(MagicColors magiccolor);
		void Update(MagicColors magiccolor);
		void Remove(MagicColors magiccolor);
		MagicColors GetById(int magicColorId);
	}
}