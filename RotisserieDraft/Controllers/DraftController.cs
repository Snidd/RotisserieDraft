﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RotisserieDraft.Logic;
using RotisserieDraft.Models;
using RotisserieDraft.ViewModels;

namespace RotisserieDraft.Controllers
{
    public class DraftController : Controller
    {
        //
        // GET: /Draft/

        public ActionResult Index()
        {
            var draftList = new List<DraftListViewModel>();

            using (var sl = new SystemLogic())
            {
                var drafts = sl.GetDraftList();
                foreach (var draft in drafts)
                {
                    var dlvm = new DraftListViewModel
                                   {
                                       CreatorId = draft.Owner.Id,
                                       CreatorName = sl.GetMember(draft.Owner.Id).FullName,
                                       Id = draft.Id,
                                       Name = draft.Name,
                                       Public = draft.Public
                                   };

                    draftList.Add(dlvm);
                }
            }

            return View(draftList);
        }

        //
        // GET: /Draft/Details/5

        public ActionResult Details(int id)
        {
            var draftLogic = GetDraftLogic.FromDraftId(id);
            var systemLogic = new SystemLogic();

            if (!draftLogic.IsDraftAvailable(id))
                return RedirectToAction("Index");

            var draft = systemLogic.GetDraftById(id);
            using (var sl = new SystemLogic())
            {
                var picks = systemLogic.GetPickList(id);
                var dvm = new DraftViewModel
                              {
                                  MaximumNumberOfPicks = draft.MaximumPicksPerMember,
                                  Name = draft.Name,
                                  Owner = sl.GetMember(draft.Owner.Id).FullName,
                                  CreationDate = draft.CreatedDate,
                                  CurrentWheelPosition = draftLogic.CurrentWheelPosition(id),
                                  CurrentPickPosition = draftLogic.CurrentPickPosition(id),
                                  CurrentNumberOfPicks = picks.Count
                              };

                var members = systemLogic.GetDraftMembers(id);

                foreach (var draftMemberPositions in members)
                {
                    var member = sl.GetMember(draftMemberPositions.Id);
                    var draftMemberVm = new DraftMemberVm
                                  {
                                      DisplayName = member.FullName,
                                      Id = member.Id,
                                  };

                    dvm.Members.Add(draftMemberVm);
                }

                var pickCount = picks.Count;
                var startIndex = pickCount - draft.DraftSize;
                if (startIndex < 0)
                    startIndex = 0;

                for (int i = startIndex; i < pickCount; i++)
                {
                    var pick = picks[i];

                    var member = sl.GetMember(pick.Member.Id);
                    var card = sl.GetCard(pick.Card.Id);

                    var pvm = new PickViewModel {CardId = pick.Card.Id, MemberId = pick.Member.Id, PickTime = PickTime.History, CardName = card.Name, MemberName = member.FullName};
                    dvm.Picks.Add(pvm);
                }

                var currentPick = new PickViewModel()
                              {
                                  MemberId = draft.CurrentTurn.Id,
                                  MemberName = sl.GetMember(draft.CurrentTurn.Id).FullName,
                                  PickTime = PickTime.Current,
                              };
                dvm.Picks.Add(currentPick);

                for (int i = 0; i < draft.DraftSize; i++)
                {
                    var nextDraftPosition = draftLogic.GetNextPickPosition(pickCount + 1 + i, draft.DraftSize);
                    var nextMember = dvm.Members[nextDraftPosition - 1];
                    var nextPick = new PickViewModel
                                        {
                                            MemberName = nextMember.DisplayName,
                                            MemberId = nextMember.Id,
                                            PickTime = PickTime.Future
                                        };

                    dvm.Picks.Add(nextPick);
                }

            return View(dvm);
            }
        }

        //
        // GET: /Draft/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Draft/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Draft/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Draft/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Draft/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Draft/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
