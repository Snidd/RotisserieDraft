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
        private Member GetAuthorizedMember()
        {
            using (var sl = new SystemLogic())
            {
                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    var userName = System.Web.HttpContext.Current.User.Identity.Name;
                    var member = sl.GetMember(userName);
                    if (member != null)
                        return member;
                }
                return null;
            }
        }

        public ActionResult Google(int id)
        {
            return View(GetDraftViewModel(id));
        }

        public ActionResult Index()
        {
            var draftList = new List<DraftListViewModel>();

            using (var sl = new SystemLogic())
            {
                var authMember = GetAuthorizedMember();

                var drafts = sl.GetDraftList();
                foreach (var draft in drafts)
                {
                    var dlvm = new DraftListViewModel
                                   {
                                       CreatorId = draft.Owner.Id,
                                       CreatorName = sl.GetMember(draft.Owner.Id).FullName,
                                       Id = draft.Id,
                                       Name = draft.Name
                                   };
					if (authMember != null)
					{
						dlvm.AmIMemberOf = sl.IsMemberOfDraft
							(authMember.Id, draft.Id);
					}

                    draftList.Add(dlvm);
                }
            }

            return View(draftList);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Pick(string cardName, int draftId)
        {
            var authMember = GetAuthorizedMember();

            using (var sl = new SystemLogic())
            {
                if (!sl.IsMemberOfDraft(authMember.Id, draftId)) return RedirectToAction("Index");

                var draftLogic = GetDraftLogic.FromDraftId(draftId);
                var card = sl.GetCard(cardName);

                if (card == null)
                {
                    var likeList = sl.FindCard(cardName);
                    return Json(new { pickresult = false, reason = "Card was not found!", alternatives = likeList });
                }

                var pickSuccess = draftLogic.PickCard(draftId, authMember.Id, card.Id);

                if (!pickSuccess)
                {
                    return Json(new { pickresult = false, reason = "Card was already picked, try another card!" });
                }

                DraftViewModel dvm = GetDraftViewModel(draftId);

                return Json(new { pickresult = true, updatedDvm = dvm });
            }
        }

        private DraftViewModel GetDraftViewModel(int draftId)
        {
            var draftLogic = GetDraftLogic.FromDraftId(draftId);
            var systemLogic = new SystemLogic();

            if (!draftLogic.IsDraftAvailable(draftId))
                return null;

            var draft = systemLogic.GetDraftById(draftId);
            using (var sl = new SystemLogic())
            {
                var picks = systemLogic.GetPickList(draftId);
                var dvm = new DraftViewModel
                {
                    Id = draftId,
                    MaximumNumberOfPicks = draft.MaximumPicksPerMember,
                    Name = draft.Name,
                    Owner = sl.GetMember(draft.Owner.Id).FullName,
                    CreationDate = draft.CreatedDate,
                    CurrentPickPosition = draftLogic.CurrentPickPosition(draftId),
                    CurrentNumberOfPicks = picks.Count
                };

                var members = systemLogic.GetDraftMembers(draftId);

                foreach (var draftMemberPositions in members)
                {
                    var member = sl.GetMember(draftMemberPositions.Member.Id);
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

                    var pvm = new PickViewModel { CardId = pick.Card.Id, MemberId = pick.Member.Id, PickTime = PickTime.History, CardName = card.Name, MemberName = member.FullName };
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

                var authMember = GetAuthorizedMember();
                if (authMember != null)
                {
                    foreach (FuturePick fp in sl.GetMyFuturePicks(draftId, authMember.Id))
                    {
                        Card card = sl.GetCard(fp.Card.Id);
                        dvm.FuturePicks.Add(card.Name);
                    }
                }

                return dvm;
            }
        }

		/// <summary>
		/// Removes a pick for the authorized member, for now only removes from futurepicks, will be extended later with possibility to remove a pick if next player hasnt picked.
		/// </summary>
		/// <param name="cardName"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize]
		public ActionResult RemovePick(int id, string cardName)
		{
			var member = GetAuthorizedMember();

			if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

			using (var sl = new SystemLogic())
			{
				if (!sl.IsMemberOfDraft(member.Id, id)) return RedirectToAction("Index");

				List<FuturePick> futurePicks = sl.GetMyFuturePicks(id, member.Id);

				foreach (var futurePick in futurePicks)
				{
					var card = sl.GetCard(futurePick.Card.Id);
					if (card.Name.Equals(cardName))
					{
						sl.RemoveMyFuturePick(futurePick.Id);
						return Json(new {dvm = GetDraftViewModel(id), success = true}, JsonRequestBehavior.AllowGet);
					}
				}

				return Json(new {success = false}, JsonRequestBehavior.AllowGet);
			}
		}

		//
		// GET: /Draft/Details/5
		public ActionResult Details(int id)
        {
            DraftViewModel dvm = GetDraftViewModel(id);
            if (dvm == null) return RedirectToAction("Index");

			var member = GetAuthorizedMember();
			if (member != null)
			{
				using (var sl = new SystemLogic())
				{
					dvm.IsMemberOfDraft = sl.IsMemberOfDraft(member.Id, id);
				}
			}

			if (Request.IsAjaxRequest())
			{
				return Json(dvm, JsonRequestBehavior.AllowGet);
			}

            return View(dvm);
        }

        //
        // GET: /Draft/Create

		[Authorize]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Draft/Create

		[Authorize]
        [HttpPost]
        public ActionResult Create(CreateDraftViewModel model)
        {
            try
            {
            	IDraftLogic draftLogic = GetDraftLogic.DefaultDraftLogic();

				using (var sl = new SystemLogic())
				{
					var draft = draftLogic.CreateDraft(model.DraftName, GetAuthorizedMember().Id, model.MaximumNumberOfPicks, model.IsPublic);

                    return RedirectToAction("Start", new { id = draft.Id });
				}
            }
            catch
            {
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddMember(string memberIdentification, int id)
        {
            using (var sl = new SystemLogic())
            {
                var authMember = GetAuthorizedMember();
                var draft = sl.GetDraftById(id);

                if (draft.Owner.Id != authMember.Id)
                    return RedirectToAction("Index");

                var dl = GetDraftLogic.FromDraft(draft);

                var member = sl.FindMember(memberIdentification);
                if (member != null)
                {
                    dl.AddMemberToDraft(draft.Id, member.Id);
                }

                return RedirectToAction("Start", new {id = draft.Id});
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Start(StartDraftViewModel model)
        {
            using (var sl = new SystemLogic())
            {
                var authMember = GetAuthorizedMember();
                var draft = sl.GetDraftById(model.DraftId);

                if (draft.Owner.Id != authMember.Id || draft.Started || draft.Finished)
                    return RedirectToAction("Index");

                var dl = GetDraftLogic.FromDraft(draft);
                dl.StartDraft(draft.Id, model.RandomizeSeats);

                return RedirectToAction("Details", new { id = draft.Id });
            }
        }

        [Authorize]
        public ActionResult Start(int id)
        {
            using (var sl = new SystemLogic())
            {
                var authMember = GetAuthorizedMember();
                var draft = sl.GetDraftById(id);

                if (draft.Owner.Id != authMember.Id || draft.Started || draft.Finished)
                    return RedirectToAction("Index");

                var startDraftVm = new StartDraftViewModel {DraftName = draft.Name, DraftId = draft.Id };
                var draftMemberPositions = sl.GetDraftMembers(draft.Id);

                foreach (var dfs in draftMemberPositions)
                {
                    var member = sl.GetMember(dfs.Member.Id);
                    startDraftVm.DraftMembers.Add(new DraftMemberViewModel
                                                      {
                                                          DraftPosition = dfs.Position,
                                                          Email = member.Email,
                                                          FullName = member.FullName
                                                      });
                }

                return View(startDraftVm);
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
