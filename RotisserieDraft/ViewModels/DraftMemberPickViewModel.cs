using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RotisserieDraft.ViewModels
{
    public class DraftMemberPickViewModel
    {
        public DraftMemberPickViewModel()
        {
            Picks = new List<PickViewModel>();
            ManaCurve = new List<int>();
            ColorPercentages = new List<string>();
        }

        public int NumberOfPicks { get; set; }
        public List<PickViewModel> Picks { get; set; }
        public List<int> ManaCurve { get; set; }
        public List<string> ColorPercentages { get; set; }
        public string ManaCurveSparkLine { get; set; }
        public string ManaCurveDescriptionLine { get; set; }

        public string ColorsSparkLine { get; set; }
        public string ColorsSparkColorArray { get; set; }
    }
}