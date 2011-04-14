using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RotisserieDraft.Models;

namespace RotisserieDraft.Domain
{
	public interface ICardRepository
	{
		void Add(Card card);
		void Update(Card card);
		void Remove(Card card);

		Card GetById(int cardId);
		Card GetByName(string name);

		Card FindCard(string searchtext);
		Card FindCard(string searchtext, ICollection<MagicColor> colors);
	}
}