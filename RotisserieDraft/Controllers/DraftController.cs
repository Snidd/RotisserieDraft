using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RotisserieDraft.Logic;
using RotisserieDraft.Models;
using RotisserieDraft.ViewModels;
using RotisserieDraft.Util;

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
            return View(GetDraftViewModel(id, false));
        }

		public ActionResult MemberStatistics(int draftId, int memberId)
		{
			using (var sl = new SystemLogic())
			{
				var draft = sl.GetDraftById(draftId);
				
				if (!draft.Public)
				{
					var member = GetAuthorizedMember();
					if (member == null || !sl.IsMemberOfDraft(member.Id, draftId))
					{
						if (Request.IsAjaxRequest()) return Json(new {success = false});
							
						return RedirectToAction("Index");
					}
				}

				var res = new DraftMemberPickViewModel();

				var pickList = sl.GetPickList(draftId, memberId);

				int artifactPicks = 0;
				int bluePicks = 0;
				int redPicks = 0;
				int whitePicks = 0;
				int greenPicks = 0;
				int blackPicks = 0;
				int landPicks = 0;

				res.NumberOfPicks = pickList.Count;

				foreach (var pick in pickList)
				{
					var card = sl.GetCard(pick.Card.Id);

                    PickViewModel pvm = new PickViewModel() { ColorClass = GetColorClass(card), CardName = card.Name, CardId = card.Id };
					res.Picks.Add(pvm);

					if (card.IsBlack()) blackPicks++;
					if (card.IsGreen()) greenPicks++;
					if (card.IsRed()) redPicks++;
					if (card.IsBlue()) bluePicks++;
					if (card.IsWhite()) whitePicks++;
					if (card.IsArtifact()) artifactPicks++;
					if (card.IsLand()) landPicks++;

					int convertedManaCost = card.GetConvertedManaCost();
					while (convertedManaCost >= res.ManaCurve.Count)
					{
						res.ManaCurve.Add(0);
					}
					res.ManaCurve[convertedManaCost]++;
				}

                /*
				if (landPicks > 0) res.ColorPercentages.Add(GetPercentageText("Lands", landPicks, res.NumberOfPicks));
				if (greenPicks > 0) res.ColorPercentages.Add(GetPercentageText("Green", greenPicks, res.NumberOfPicks));
				if (redPicks > 0) res.ColorPercentages.Add(GetPercentageText("Red", redPicks, res.NumberOfPicks));
				if (bluePicks > 0) res.ColorPercentages.Add(GetPercentageText("Blue", bluePicks, res.NumberOfPicks));
				if (blackPicks > 0) res.ColorPercentages.Add(GetPercentageText("Black", blackPicks, res.NumberOfPicks));
				if (whitePicks > 0) res.ColorPercentages.Add(GetPercentageText("White", whitePicks, res.NumberOfPicks));
				if (artifactPicks > 0) res.ColorPercentages.Add(GetPercentageText("Artifacts", artifactPicks, res.NumberOfPicks));
                */

                if (greenPicks > 0)
                {                    
                    res.ColorsSparkLine += greenPicks + ",";
                    res.ColorsSparkColorArray += "'green',";
                }

                if (redPicks > 0)
                {
                    res.ColorsSparkLine += redPicks + ",";
                    res.ColorsSparkColorArray += "'red',";
                }

                if (bluePicks > 0)
                {
                    res.ColorsSparkLine += bluePicks + ",";
                    res.ColorsSparkColorArray += "'blue',";
                }

                if (blackPicks > 0)
                {
                    res.ColorsSparkLine += blackPicks + ",";
                    res.ColorsSparkColorArray += "'black',";
                }

                if (whitePicks > 0)
                {
                    res.ColorsSparkLine += redPicks + ",";
                    res.ColorsSparkColorArray += "'white',";
                }

                if (artifactPicks > 0)
                {
                    res.ColorsSparkLine += redPicks + ",";
                    res.ColorsSparkColorArray += "'brown',";
                }
                
                int count = 0;
                foreach (int manacurve in res.ManaCurve)
                {
                    res.ManaCurveSparkLine += manacurve + ",";
                    res.ManaCurveDescriptionLine += count + " ";
                    count++;
                }
                
                res.ManaCurveDescriptionLine = string.IsNullOrEmpty(res.ManaCurveDescriptionLine) ? "" : res.ManaCurveDescriptionLine.Trim();
                res.ManaCurveSparkLine = string.IsNullOrEmpty(res.ManaCurveSparkLine) ? "" : res.ManaCurveSparkLine.Trim(',');
                res.ColorsSparkColorArray = string.IsNullOrEmpty(res.ColorsSparkColorArray) ? "" : res.ColorsSparkColorArray.Trim(',');
                res.ColorsSparkLine = string.IsNullOrEmpty(res.ColorsSparkLine) ? "" : res.ColorsSparkLine.Trim(',');

                if (Request.IsAjaxRequest()) return Json(new {success = true, result = res});

                return View(res);
			}
		}

		private static string GetPercentageText(string type, int picksOfTheType, int totalPicks)
		{
			double percentage = picksOfTheType/totalPicks*100;
			percentage = Math.Round(percentage, 1);

			return type + ": " + picksOfTheType + "(" + percentage + ")";
		}

        public ActionResult Index()
        {
            var dlvm = new DraftListViewModel();

            using (var sl = new SystemLogic())
            {
                var authMember = GetAuthorizedMember();

                var draftIdsAdded = new List<int>();

                if (authMember != null)
                {
                    var myDrafts = sl.GetDraftListFromMember(authMember.Id);
                    foreach (var draft in myDrafts)
                    {
                        var owner = sl.GetMember(draft.Owner.Id);
                        dlvm.MyDrafts.Add(new DraftListDetails { CreatorId = owner.Id, CreatorName = owner.FullName, Id = draft.Id, Name = draft.Name });
                        draftIdsAdded.Add(draft.Id);
                    }
                }

                var drafts = sl.GetPublicDraftList();
                foreach (var draft in drafts)
                {
                    if (draftIdsAdded.Contains(draft.Id)) continue;
                    var owner = sl.GetMember(draft.Owner.Id);
                    dlvm.PublicDrafts.Add(new DraftListDetails { CreatorId = owner.Id, CreatorName = owner.FullName, Id = draft.Id, Name = draft.Name});
                }


            }

            return View(dlvm);
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

                DraftViewModel dvm = GetDraftViewModel(draftId, false);

                return Json(new { pickresult = true, updatedDvm = dvm });
            }
        }

        private DraftViewModel GetDraftViewModel(int draftId, bool allPicks)
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

                if (allPicks) startIndex = 0;

                for (int i = startIndex; i < pickCount; i++)
                {
                    var pick = picks[i];

                    var member = sl.GetMember(pick.Member.Id);
                    var card = sl.GetCard(pick.Card.Id);

                    var pvm = new PickViewModel { CardId = pick.Card.Id, MemberId = pick.Member.Id, PickTime = PickTime.History, ColorClass = GetColorClass(card), CardName = card.Name, MemberName = member.FullName };
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

        private string GetColorClass(Card card)
        {
            // Follow the color pie (WUBRG)

            string colorClass = "";

            if (card.IsWhite()) colorClass += "w";
            if (card.IsBlue()) colorClass += "u";
            if (card.IsBlack()) colorClass += "b";
            if (card.IsRed()) colorClass += "r";
            if (card.IsGreen()) colorClass += "g";

            if (card.IsLand()) colorClass = "l";
            if (card.IsArtifact()) colorClass = "a";

            if (colorClass.Length > 3) colorClass = "gold";

            return "cardcolor" + colorClass;
        }

		[Authorize]
		public ActionResult UpdateChat(int id, int lastChatId)
		{
			var member = GetAuthorizedMember();

			using (var sl = new SystemLogic())
			{
				if (!sl.IsMemberOfDraft(member.Id, id))
				{
					if (Request.IsAjaxRequest()) return Json(new {success = false});
					return RedirectToAction("Index");
				}

				var newChats = ConvertToChatViewModelList(sl.GetUpdatedChatList(id, lastChatId), member.Id);

				if (Request.IsAjaxRequest()) return Json(new { success = true, chats = newChats });
				return RedirectToAction("Details", new { id = id });
			}
		}

		private List<ChatViewModel> ConvertToChatViewModelList(IEnumerable<Chat> chatlist, int currentMemberId)
		{
			using (var sl = new SystemLogic())
			{
				var vmchats = new List<ChatViewModel>();
				foreach (Chat chat in chatlist)
				{
					var chatmember = sl.GetMember(chat.Member.Id);
					var cvm = new ChatViewModel
					{
						ChatId = chat.Id,
						DateTime = chat.CreatedDate.ToString("HH:mm"),
						MemberName = currentMemberId == chatmember.Id ? "Me" : chatmember.FullName,
						Text = chat.Text
					};
					vmchats.Add(cvm);
				}

				return vmchats;
			}
		}

		[Authorize]
		public ActionResult ListChat(int id)
		{
			var member = GetAuthorizedMember();

			using (var sl = new SystemLogic())
			{
				if (!sl.IsMemberOfDraft(member.Id, id))
				{
					if (Request.IsAjaxRequest()) return Json(new { success = false });
					return RedirectToAction("Index");
				}

				var chats = sl.GetChatList(id);

				var vmchats = ConvertToChatViewModelList(chats, member.Id);

				if (Request.IsAjaxRequest()) return Json(new { success = true, chats = vmchats });
				return RedirectToAction("Details", new { id = id });
			}
		}

		[Authorize]
		[HttpPost]
		public ActionResult Chat(int draftId, string message, int chatTempId)
		{
			var member = GetAuthorizedMember();

			using (var sl = new SystemLogic())
			{
				if (!sl.IsMemberOfDraft(member.Id, draftId))
				{
					if (Request.IsAjaxRequest()) return Json(new {success = false});
					return RedirectToAction("Index");
				}

				var chat = sl.AddChat(message, draftId, member.Id);

				if (Request.IsAjaxRequest()) return Json(new {success = true, chatId = chat.Id, oldChatId = chatTempId});
				return RedirectToAction("Details", new {id = draftId});
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
						return Json(new {dvm = GetDraftViewModel(id, false), success = true}, JsonRequestBehavior.AllowGet);
					}
				}

				return Json(new {success = false}, JsonRequestBehavior.AllowGet);
			}
		}

        public ActionResult Details(int id, bool allPicks = false)
        {
            DraftViewModel dvm = GetDraftViewModel(id, allPicks);
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
		// GET: /Draft/Details/5
        /*
		public ActionResult Details(int id)
        {
            return Details(id, false);
        }
        */

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
