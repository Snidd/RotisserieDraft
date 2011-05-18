using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RotisserieDraft.ViewModels
{
    public class ChatViewModel
    {
        public int ChatId { get; set; }
        public string MemberName { get; set; }
        public string Text { get; set; }
        public string DateTime { get; set; }
    }
}