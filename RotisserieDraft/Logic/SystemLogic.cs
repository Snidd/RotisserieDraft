using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using RotisserieDraft.Domain;
using RotisserieDraft.Models;
using RotisserieDraft.Repositories;

namespace RotisserieDraft.Logic
{
    public class SystemLogic : IDisposable
    {

		public Chat AddChat(string message, int draftId, int memberId)
		{
			IChatRepository cr = new ChatRepository();
			IDraftRepository dr = new DraftRepository();
			IMemberRepository mr = new MemberRepository();

			var draft = dr.GetById(draftId);
			var member = mr.GetById(memberId);

			var chat = new Chat() {Draft = draft, Member = member, Text = message};

			cr.Add(chat);

			return chat;
		}

		public List<Chat> GetChatList(int draftId)
		{
			IChatRepository cr = new ChatRepository();
			IDraftRepository dr = new DraftRepository();

			var draft = dr.GetById(draftId);
			var res = cr.ListByDraft(draft);

			return res != null ? res.ToList() : new List<Chat>();
		}

		public List<Chat> GetUpdatedChatList(int draftId, int latestChatId)
		{
			IChatRepository cr = new ChatRepository();
			IDraftRepository dr = new DraftRepository();

			var draft = dr.GetById(draftId);
			var res = cr.ListNewChatsFromDraft(draft, latestChatId);

			return res != null ? res.ToList() : new List<Chat>();
		}

		public void RemoveChat(int chatId)
		{
			throw new NotImplementedException("Not yet!");
		}

        public Member FindMember(string memberIdentification)
        {
            IMemberRepository mr = new MemberRepository();

            var member = mr.GetByEmail(memberIdentification);
            if (member != null)
                return member;

            member = mr.GetByUsername(memberIdentification);
            if (member != null)
                return member;

            return null;
        }

        public bool CreateUser(string username, string email, string password, string fullname)
        {
            IMemberRepository mr = new MemberRepository();
            
            string passwordHash = FormsAuthentication.HashPasswordForStoringInConfigFile(
                                      password+username, "sha1");

            var member = new Member {UserName = username, Email = email, Password = passwordHash, FullName = fullname};

            try
            {
                mr.Add(member);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool AuthenticateUser(string username, string password)
        {
            IMemberRepository mr = new MemberRepository();
            Member member = mr.GetByUsername(username);
            if (member == null)
                return false;

            string passwordHash = FormsAuthentication.HashPasswordForStoringInConfigFile(password+username, "sha1");
            return member.Password == passwordHash;
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            IMemberRepository mr = new MemberRepository();
            Member member = mr.GetByUsername(username);
            if (member == null)
                return false;

            string oldPasswordHash = FormsAuthentication.HashPasswordForStoringInConfigFile(oldPassword, "sha1");
            if (member.Password != oldPasswordHash)
                return false;

            string newPasswordHash = FormsAuthentication.HashPasswordForStoringInConfigFile(newPassword, "sha1");
            member.Password = newPasswordHash;
            try
            {
                mr.Update(member);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public Draft GetDraftById(int draftId)
        {
            IDraftRepository dr = new DraftRepository();
            return dr.GetById(draftId);
        }

        public bool IsCardPicked(int draftId, int cardId)
        {
            IPickRepository pr = new PickRepository();
            IDraftRepository dr = new DraftRepository();
            ICardRepository cr = new CardRepository();

            var draft = dr.GetById(draftId);
            var card = cr.GetById(cardId);

            var pick = pr.GetPickByCardAndDraft(card, draft);
            return pick != null;
        }

        public List<Pick> GetLatestPicksByPlayer(int draftId, int memberId)
        {
            IDraftRepository dr = new DraftRepository();
            IPickRepository pr = new PickRepository();
            IMemberRepository mr = new MemberRepository();

            var draft = dr.GetById(draftId);
            var member = mr.GetById(memberId);
            var picksCollection = pr.GetPicksByDraftAndMember(draft, member);

            if (picksCollection == null)
                return new List<Pick>();

            var picks = picksCollection.ToList();
            picks.Sort((p1, p2) => p1.CreatedDate.CompareTo(p2.CreatedDate));

            return picks;
        }

        public List<FuturePick> GetMyFuturePicks(int draftId, int memberId)
        {
            IFuturePickRepository fpr = new FuturePickRepository();
            IDraftRepository dr = new DraftRepository();
            IMemberRepository mr = new MemberRepository();

            var draft = dr.GetById(draftId);
            var member = mr.GetById(memberId);
            var ret = fpr.GetFuturePicksByDraftAndMember(draft, member);
            return ret == null ? new List<FuturePick>() : ret.ToList();
        }

        public List<DraftMemberPositions> GetDraftMembers(int draftId)
        {
            IDraftMemberPositionsRepository dmpr = new DraftMemberPositionsRepository();
            IDraftRepository dr = new DraftRepository();

            var draft = dr.GetById(draftId);

            var ret = dmpr.GetMemberPositionsByDraft(draft);
            return ret == null ? new List<DraftMemberPositions>() : ret.ToList();
        }

        public List<Card> FindCard(string searchText)
        {
            ICardRepository cr = new CardRepository();
            var ret = cr.FindCard(searchText);
            return ret == null ? new List<Card>() : ret.ToList();
        }

        public Card GetCard(string cardName)
        {
            ICardRepository cr = new CardRepository();
            return cr.GetByName(cardName);
        }

		public bool IsMemberOfDraft(int memberId, int draftId)
		{
			IDraftMemberPositionsRepository dmpr = new DraftMemberPositionsRepository();
			IDraftRepository dr = new DraftRepository();

			var draft = dr.GetById(draftId);
			var memberPositions = dmpr.GetMemberPositionsByDraft(draft);

			return memberPositions.Any(memberPosition => memberPosition.Member.Id == memberId);
		}

        public Member GetMember(int memberId)
        {
            IMemberRepository mr = new MemberRepository();
            return mr.GetById(memberId);
        }

		public Member GetMember(string userName)
		{
			IMemberRepository mr = new MemberRepository();
			return mr.GetByUsername(userName);
		}

        public Card GetCard(int cardId)
        {
            ICardRepository cr = new CardRepository();
            return cr.GetById(cardId);
        }

        public void RemoveMyFuturePick(int futurePickId)
        {
            IFuturePickRepository futurePickRepository = new FuturePickRepository();
            futurePickRepository.RemoveFuturePick(new FuturePick { Id = futurePickId });
        }

        public List<Pick> GetPickList(int draftId)
        {
            IPickRepository pickRepository = new PickRepository();
            IDraftRepository dr = new DraftRepository();

            var draft = dr.GetById(draftId);
            var ret = pickRepository.GetPicksByDraft(draft);
            return ret == null ? new List<Pick>() : ret.ToList();
        }

		public List<Pick> GetPickList(int draftId, int memberId)
		{
			IPickRepository pickRepository = new PickRepository();
			IDraftRepository dr = new DraftRepository();
			IMemberRepository mr = new MemberRepository();

			var draft = dr.GetById(draftId);
			var member = mr.GetById(memberId);

			var ret = pickRepository.GetPicksByDraftAndMember(draft, member);
			return ret == null ? new List<Pick>() : ret.ToList();
		}

        public List<Draft> GetPublicDraftList()
        {
            IDraftRepository dr = new DraftRepository();
            var ret = dr.ListActive();
            return ret == null ? new List<Draft>() : ret.ToList();
        }

        public List<Draft> GetDraftListFromMember(int memberId)
        {
            IDraftMemberPositionsRepository dmpr = new DraftMemberPositionsRepository();
            IMemberRepository mr = new MemberRepository();
            IDraftRepository dr = new DraftRepository();

            var member = mr.GetById(memberId);
            
            var draftList = new List<Draft>();

            var draftPositions = dmpr.GetDraftPositionsByMember(member);
            if (draftPositions == null) return draftList;

            foreach (DraftMemberPositions dmp in draftPositions)
            {
                draftList.Add(dr.GetById(dmp.Draft.Id));
            }

            return draftList;
        }

        public void Dispose()
        {
            
        }
    }
}