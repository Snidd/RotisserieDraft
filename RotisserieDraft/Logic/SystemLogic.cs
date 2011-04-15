﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RotisserieDraft.Domain;
using RotisserieDraft.Models;
using RotisserieDraft.Repositories;

namespace RotisserieDraft.Logic
{
    public class SystemLogic : IDisposable
    {
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

        public Member GetMember(int memberId)
        {
            IMemberRepository mr = new MemberRepository();
            return mr.GetById(memberId);
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
            var ret = dr.List();
            return ret == null ? new List<Draft>() : ret.ToList();
        }

        public void Dispose()
        {
            
        }
    }
}