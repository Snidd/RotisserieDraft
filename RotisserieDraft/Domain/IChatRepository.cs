using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RotisserieDraft.Models;

namespace RotisserieDraft.Domain
{
	public interface IChatRepository
	{
		void Add(Chat chat);
		void Remove(Chat chat);

		ICollection<Chat> ListByDraft(Draft draft);
		ICollection<Chat> ListNewChatsFromDraft(Draft draft, int latestChatId);
	}
}