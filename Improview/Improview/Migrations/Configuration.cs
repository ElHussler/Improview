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

            var questionsGamesProgrammer = new List<Question>
            {
                new Question{Text="What games are you playing at the moment?",
                             Type=QType.Candidate,Number=1},
                new Question{Text="What's the main thing that made you apply for this position at Naughty Dog?",
                             Type=QType.Employer,Number=2},
                new Question{Text="What will you bring to the team?",
                             Type=QType.Cliche,Number=3},
                new Question{Text="Thinking of your favourite game, how would you apply the skills you've acquired to make it 'better'?",
                             Type=QType.Technical,Number=4},
                new Question{Text="What do you do on your own time to extend your skills?",
                             Type=QType.Technical,Number=5}
            };

            var interviewDeveloper = new Interview
            {
                Questions = questionsDeveloper,
                Title = "Software Developer C#/.NET/SQL",
                Description = "Company: XD Decisions. Developing web-connected applications with SQL backends, degree or 2 years experience desirable"
            };

            var interviewLawyer = new Interview
            {
                Questions = questionsLawyer,
                Title = "Junior Criminal Practice Lawyer",
                Description = "Company: HHM. Graduate position for Criminal Law post-grads, hands-on client case work assisting industry leaders"
            };

            var interviewGameProgrammer = new Interview
            {
                Questions = questionsGamesProgrammer,
                Title = "Junior Games Programmer",
                Description = "Company: Naughty Dog. Junior/Graduate position learning from and developing with some of the most seasoned programmers in the business"
            };

            var interviewsDeveloperLawyerProgrammer = new List<Interview>
            {
                interviewDeveloper,
                interviewLawyer,
                interviewGameProgrammer
            };

            var testAnswerDeveloper = new Answer
            {
                AnswerID = 49,
                FilePathServerRelative = "~/Uploads/169880863.webm",
                FilePathServerAbsolute = "C:/Users/Luke/Dropbox/Source/MPrepo/Improview/Improview/Uploads/169880863.webm",
                FilePathAzureBlobStorage = "",
                Interview = interviewDeveloper,
                IsRecorded = true,
                Reviews = null,
                Number = 1,
                Rating = 2,
                UserID = "5af558b1-23cb-45b6-ab2d-d42a1d84f161"
            };

            var testReviewDeveloper = new Review
            {
                Rating = 3,
                Comment = "Very good eye contact and body language, however you need to practice your answer delivery as it lacks structure making it hard to follow",
                AnswerID = 49,
                Answer = testAnswerDeveloper
            };

            questionsDeveloper.ForEach(q => context.Questions.AddOrUpdate(p => p.Text, q));
            questionsLawyer.ForEach(q => context.Questions.AddOrUpdate(p => p.Text, q));
            interviewsDeveloperLawyerProgrammer.ForEach(i => context.Interviews.AddOrUpdate(j => j.Description, i));
            context.Answers.AddOrUpdate(j => j.FilePathServerRelative, testAnswerDeveloper);
            context.Reviews.AddOrUpdate(j => j.Comment, testReviewDeveloper);

            context.SaveChanges();
        }
    }
}
