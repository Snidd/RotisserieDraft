using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RotisserieDraft.ViewModels
{
    public class DraftListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Public { get; set; }
        public string CreatorName { get; set; }
        public int CreatorId { get; set; }
    }
}