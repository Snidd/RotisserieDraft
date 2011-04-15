using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RotisserieDraft.Models;

namespace RotisserieDraft.Domain
{
	public interface IDraftRepository
	{
		void Add(Draft draft);
		void Update(Draft draft);
		void Remove(Draft draft);
		Draft GetById(int draftId);
		ICollection<Draft> GetByName(string name);
	    ICollection<Draft> List();

	}
}