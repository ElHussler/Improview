using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Improview.Models;
using System.ComponentModel.DataAnnotations;

namespace Improview.Models
{
    public class Review
    {
        public int ReviewID { get; set; }

        [Required]
        [Display(Name = "Add Rating")]
        [Range(0, 5)]
        public int Rating { get; set; }

        [Required]
        [Display(Name = "Add Comment")]
        [StringLength(255, MinimumLength = 1)]
        public string Comment { get; set; }

        [Required]
        public int AnswerID { get; set; }

        public virtual Answer Answer { get; set; }
    }
}