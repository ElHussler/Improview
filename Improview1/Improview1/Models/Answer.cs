using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Improview1.Models
{
    public class Answer
    {
        public int AnswerID { get; set; }

        [Range(1, 15)]
        public int Number { get; set; }

        [Required]
        public bool IsRecorded { get; set; }

        [Display(Name = "Path to answer video on server")]
        public string FilePath { get; set; }

        [Required]
        public string UserID { get; set; }

        public virtual Interview Interview { get; set; }
        public virtual ICollection<Review> Review { get; set; }
    }
}