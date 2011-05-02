using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RotisserieDraft.ViewModels
{

    public class DraftMemberVm
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
    }

    public partial class DraftViewModel
    {
        public DraftViewModel()
        {
            Members = new List<DraftMemberVm>();
            Picks = new List<PickViewModel>();
            FuturePicks = new List<string>();
        }

        public int Id { get; set; }
        public List<DraftMemberVm> Members { get; set; }
        public string Name { get; set; }

        public List<string> FuturePicks { get; set; }
        
        public int CurrentNumberOfPicks { get; set; }
        public int MaximumNumberOfPicks { get; set; }
        public int CurrentPickPosition { get; set; }
        
        public string Owner { get; set; }
        public DateTime CreationDate { get; set; }

        public List<PickViewModel> Picks;
    }
}