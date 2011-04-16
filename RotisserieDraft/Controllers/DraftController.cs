using System;
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
                var dvm = new DraftViewModel
                              {
                                  MaximumNumberOfPicks = draft.MaximumPicksPerMember,
                                  Name = draft.Name,
                                  Owner = sl.GetMember(draft.Owner.Id).FullName,
                                  CreationDate = draft.CreatedDate,
                                  CurrentWheelPosition = draftLogic.CurrentWheelPosition(id),
                                  CurrentPickPosition = draftLogic.CurrentPickPosition(id),
                                  CurrentNumberOfPicks = systemLogic.GetPickList(id).Count
                              };

                var members = systemLogic.GetDraftMembers(id);

                foreach (var draftMemberPositions in members)
                {
                    var member = sl.GetMember(draftMemberPositions.Id);
                    var draftMemberVm = new DraftMemberVm
                                  {
                                      DisplayName = member.FullName,
                                      Id = member.Id,
                                      DraftPosition = draftMemberPositions.Position,
                                  };

                    var latestPicks = systemLogic.GetLatestPicksByPlayer(id, draftMemberVm.Id);
                    int latestPickCounter = 0;
                    foreach (var latestPick in latestPicks)
                    {
                        draftMemberVm.LastThreePicks.Add(sl.GetCard(latestPick.Card.Id).Name);
                        latestPickCounter++;
                        if (latestPickCounter >= 3)
                            break;
                    }

                    dvm.Members.Add(draftMemberVm);
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
