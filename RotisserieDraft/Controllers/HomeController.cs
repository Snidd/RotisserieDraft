using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RotisserieDraft.Logic;
using RotisserieDraft.ViewModels;

namespace RotisserieDraft.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
            using (var sl = new SystemLogic())
            {
                HomeViewModel hvm = new HomeViewModel();

                var members = sl.GetLatestMembers(5);
                foreach (var member in members)
                {
                    hvm.LatestMembers.Add(member.FullName);
                }

                return View(hvm);
            }
		}

		public ActionResult About()
		{
			return View();
		}
	}
}
