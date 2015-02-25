using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Improview1.Models
{
    public class Interview
    {
        [Key]
        [Display(Name = "Interview")]
        public int InterviewID { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Description { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}