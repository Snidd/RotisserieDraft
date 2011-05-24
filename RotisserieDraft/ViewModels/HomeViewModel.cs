using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RotisserieDraft.ViewModels
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            LatestMembers = new List<string>();
        }

        public List<string> LatestMembers { get; set; }
    }
}