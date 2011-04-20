using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RotisserieDraft.ViewModels
{
	public partial class CreateDraftViewModel
	{

		[Required]
		[Display(Name = "Draft name")]
		public string DraftName { get; set; }

		[Required]
		[Display(Name = "Show to public?")]
		public bool IsPublic { get; set; }

		[Required]
		[Display(Name = "Number of picks per member")]
		public int MaximumNumberOfPicks { get; set; }

	}
}