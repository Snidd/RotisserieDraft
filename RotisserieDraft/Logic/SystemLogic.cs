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
                                      password, "sha1");

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

            string passwordHash = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "sha1");
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

            var picks = pr.GetPicksByCardAndDraft(card, draft);
            return picks.Count > 0;
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

        public List<Draft> GetDraftList()
        {
            IDraftRepository dr = new DraftRepository();
            var ret = dr.ListActive();
            return ret == null ? new List<Draft>() : ret.ToList();
        }

        public void Dispose()
        {
            
        }
    }
}