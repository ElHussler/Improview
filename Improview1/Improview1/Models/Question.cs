using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Improview1.Models
{
    public enum QType { Employer, Candidate, Technical, Cliche }

    public class Question
    {
        public int QuestionID { get; set; }

        [Required]
        [Display(Name = "Question")]
        [StringLength(100, MinimumLength = 1)]
        public string Text { get; set; }

        [Range(1, 15)]
        public int Number { get; set; }

        [Required]
        public QType Type { get; set; }

        [Required]
        public bool IsRecorded { get; set; }

        //public Interview Interview { get; set; }

        public virtual ICollection<Interview> Interviews { get; set; }
    }
}