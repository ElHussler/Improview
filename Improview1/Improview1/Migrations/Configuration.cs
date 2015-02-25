namespace Improview1.Migrations
{
    using Improview1.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Improview1.DAL.InterviewContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Improview1.DAL.InterviewContext";
        }

        protected override void Seed(Improview1.DAL.InterviewContext context)
        {
            var questions = new List<Question>
            {
                new Question{Text="Can you tell me why you applied for this position?",
                             Type=QType.Candidate,Number=1},
                new Question{Text="What particularly attracted you to working here?",
                             Type=QType.Employer,Number=2},
                new Question{Text="Where do you see yourself in five years?",
                             Type=QType.Cliche,Number=3},
                new Question{Text="What does the virtual keyword do in C# .NET?",
                             Type=QType.Technical,Number=4},
                new Question{Text="How would you perform an inner join in SQL Server?",
                             Type=QType.Technical,Number=5}
            };
            
            var questions2 = new List<Question>
            {
                new Question{Text="What do you feel qualifies you to take up this position?",
                             Type=QType.Candidate,Number=1},
                new Question{Text="How much do you know about us?",
                             Type=QType.Employer,Number=2},
                new Question{Text="What is your biggest weakness?",
                             Type=QType.Cliche,Number=3},
                new Question{Text="How would you approach a client regarding an issue with payment?",
                             Type=QType.Technical,Number=4},
                new Question{Text="When would you deem it necessary to intervene in a direct client/police interaction?",
                             Type=QType.Technical,Number=5}
            };

            questions.ForEach(q => context.Questions.AddOrUpdate(p => p.Text, q));
            questions2.ForEach(q => context.Questions.AddOrUpdate(p => p.Text, q));
            context.SaveChanges();

            var interviews = new List<Interview>
            {
                new Interview{Questions=questions, Title="Software Developer C#/.NET/SQL", 
                              Description="Developing web-connected applications with SQL backends, degree or 2 years experience desirable"},
                new Interview{Questions=questions2, Title="Junior Criminal Practice Lawyer",
                              Description="Graduate position for Criminal Law post-grads, hands-on client case work assisting industry leaders"},
                new Interview{Questions=questions, Title="Software Developer C#/.NET/SQL", 
                              Description="Developing web-connected applications with SQL backends, degree or 2 years experience desirable 2"},
                new Interview{Questions=questions2, Title="Junior Criminal Practice Lawyer",
                              Description="Graduate position for Criminal Law post-grads, hands-on client case work assisting industry leaders 2"}
            };

            interviews.ForEach(i => context.Interviews.AddOrUpdate(j => j.Description, i));
            context.SaveChanges();
        }
    }
}
