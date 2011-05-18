using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RotisserieDraft.ViewModels
{
    public class DraftListDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string CreatorName { get; set; }
        public int CreatorId { get; set; }
    }
    public class DraftListViewModel
    {
        public DraftListViewModel()
        {
            PublicDrafts = new List<DraftListDetails>();
            MyDrafts = new List<DraftListDetails>();
        }

        public List<DraftListDetails> PublicDrafts;
        public List<DraftListDetails> MyDrafts;
    }
}