using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Improview.Models;

namespace Improview.DAL
{
    public class InterviewInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<InterviewContext>
    {
        protected override void Seed(InterviewContext context)
        {
            var questions = new List<Question>
            {
                new Question{Text="Can you tell me why you applied for this position?",
                             Type=QType.Candidate,Number=1,IsRecorded=false},
                new Question{Text="What particularly attracted you to working here?",
                             Type=QType.Employer,Number=2,IsRecorded=false},
                new Question{Text="Where do you see yourself in five years?",
                             Type=QType.Cliche,Number=3,IsRecorded=false},
                new Question{Text="What does the virtual keyword do in C# .NET?",
                             Type=QType.Technical,Number=4,IsRecorded=false},
                new Question{Text="How would you perform an inner join in SQL Server?",
                             Type=QType.Technical,Number=5,IsRecorded=false}
            };

            questions.ForEach(q => context.Questions.Add(q));
            context.SaveChanges();

            var interviews = new List<Interview>
            {
                new Interview{Questions=questions}
            };

            interviews.ForEach(i => context.Interviews.Add(i));
            context.SaveChanges();
        }
    }
}