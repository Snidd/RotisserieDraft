using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RotisserieDraft.ViewModels
{
    public enum PickTime
    {
        History = -1,
        Current = 0,
        Future = 1,
    }
    public class PickViewModel
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public int CardId { get; set; }
        public string CardName { get; set; }
        public PickTime PickTime { get; set; }
        public string ColorClass { get; set; }
    }
}