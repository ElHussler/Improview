using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Improview1.Models
{
    public class SubmitViewModel
    {
        [Required]
        [Display(Name = "Review Rating")]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [Display(Name = "Review Comment")]
        [StringLength(255, MinimumLength = 1)]
        public string Comment { get; set; }
    }
}