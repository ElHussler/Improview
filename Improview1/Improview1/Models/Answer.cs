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
        [Display(Name = "Answer Number")]
        public int Number { get; set; }

        [Required]
        public bool IsRecorded { get; set; }

        [Display(Name = "Answer Video")]
        public string FilePathServerRelative { get; set; }

        public string FilePathServerAbsolute { get; set; }
        
        public string FilePathAzureBlobStorage { get; set; }

        [Range(0, 5)]
        [Display(Name = "Rating")]
        public int Rating { get; set; }

        [Required]
        public string UserID { get; set; }

        public virtual Interview Interview { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}