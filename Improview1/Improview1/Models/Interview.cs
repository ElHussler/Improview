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
        [Display(Name = "Description")]
        [StringLength(100, MinimumLength = 1)]
        public string Description { get; set; }

        //public Question Question1 { get; set; }
        //public Question Question2 { get; set; }
        //public Question Question3 { get; set; }
        //public Question Question4 { get; set; }
        //public Question Question5 { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}