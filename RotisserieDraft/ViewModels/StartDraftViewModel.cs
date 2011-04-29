using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RotisserieDraft.ViewModels
{
    public class StartDraftViewModel
    {
        [Display(Name = "Draft name")]
        public string DraftName { get; set; }

        public int DraftId { get; set; }

        public List<DraftMemberViewModel> DraftMembers { get; set; }

        [Display(Name = "Randomize seats when starting?")]
        public bool RandomizeSeats { get; set; }

        public StartDraftViewModel()
        {
            DraftMembers = new List<DraftMemberViewModel>();
        }
        
    }
}