namespace Improview.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Collections.Generic;
    using System.Linq;
    using Improview.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Improview.DAL.InterviewContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Improview.DAL.InterviewContext";
        }

        protected override void Seed(Improview.DAL.InterviewContext context)
        {
            var questionsDeveloper = new List<Question>
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

            var questionsLawyer = new List<Question>
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

            var interviewDeveloper = new Interview
            {
                Questions = questionsDeveloper,
                Title = "Software Developer C#/.NET/SQL",
                Description = "Developing web-connected applications with SQL backends, degree or 2 years experience desirable"
            };

            var interviewLawyer = new Interview
            {
                Questions = questionsLawyer,
                Title = "Junior Criminal Practice Lawyer",
                Description = "Graduate position for Criminal Law post-grads, hands-on client case work assisting industry leaders"
            };

            var interviewsDeveloperLawyer = new List<Interview>
            {
                interviewDeveloper,
                interviewLawyer
            };

            var testAnswerDeveloper = new Answer
            {
                AnswerID = 49,
                FilePathServerRelative = "~/Uploads/169880863.webm",
                Interview = interviewDeveloper,
                IsRecorded = true,
                Reviews = null,
                Number = 1,
                Rating = 2,
                UserID = "aa07e10c-6a2d-4338-ae57-0f90dcee01b5"
            };

            var testReviewDeveloper = new Review
            {
                Rating = 2,
                Comment = "lololol",
                AnswerID = 49,
                Answer = testAnswerDeveloper
            };

            questionsDeveloper.ForEach(q => context.Questions.AddOrUpdate(p => p.Text, q));
            questionsLawyer.ForEach(q => context.Questions.AddOrUpdate(p => p.Text, q));
            interviewsDeveloperLawyer.ForEach(i => context.Interviews.AddOrUpdate(j => j.Description, i));
            context.Answers.AddOrUpdate(j => j.FilePathServerRelative, testAnswerDeveloper);
            context.Reviews.AddOrUpdate(j => j.Comment, testReviewDeveloper);

            context.SaveChanges();
        }
    }
}
