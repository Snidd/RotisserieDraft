using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RotisserieDraft.Models;

namespace RotisserieDraft.Domain
{
	public interface IMemberRepository
	{
		void Add(Member member);
		void Update(Member member);
		void Remove(Member member);
		Member GetById(int memberId);
		Member GetByEmail(string email);
	    Member GetByUsername(string username);
        ICollection<Member> GetLatestMembers(int maxCount);
	}
}