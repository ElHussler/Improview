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

        [Required]
        [Display(Name = "Answer")]
        [StringLength(100, MinimumLength = 1)]
        public string Text { get; set; }

        // Stream objects, byte arrays, for saving webm to server

        [Range(1, 15)]
        public int Number { get; set; }

        [Required]
        public bool IsRecorded { get; set; }

        public string UserID { get; set; }
        public virtual Interview Interview { get; set; }
    }
}