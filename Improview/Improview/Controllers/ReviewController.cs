using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Improview.DAL;
using Improview.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Improview.Controllers
{
    public class ReviewController : Controller
    {
        private InterviewContext db = new InterviewContext();
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<User> UserManager { get; set; }

        public ReviewController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<User>(new UserStore<User>(this.ApplicationDbContext));
        }

        // GET: Review
        public ActionResult Index()
        {
            var answers = db.Answers.Include(r => r.Reviews);

            ViewBag.Title = "Browse Answers";
            ViewBag.SubTitle = "> Choose a user's response to an interview question to view and give them feedback";

            return View(answers.ToList());
        }

        public ActionResult Answer(int aId)
        {
            ViewBag.Title = "Review Answer";
            var answer = db.Answers.Include(r => r.Reviews).Single(a => a.AnswerID == aId);
            ViewBag.aId = aId;
            return View(answer);
        }

        public ActionResult MyAnswers()
        {
            if (User.Identity.IsAuthenticated)
            {
                var tempId = User.Identity.GetUserId();
                var answers = db.Answers.Include(r => r.Reviews).Where(a => a.UserID == tempId);
                //tempId = null;

                ViewBag.Title = "Your Answers";
                ViewBag.SubTitle = "> Check out all of your submitted answers and view their feedback";

                return View("Index", answers.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Review/MyAnswers" });
            }
        }

        public ActionResult Submit(int aId)
        {
            ViewBag.Title = "Add Review";
            ViewBag.aId = aId;
            return View();
        }

        [HttpPost]
        public ActionResult Submit(SubmitViewModel model, int aId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Answer answerToUpdate = db.Answers.Find(aId);

            Review reviewToInsert = new Review();
            reviewToInsert.Rating = model.Rating;
            reviewToInsert.Comment = model.Comment;
            reviewToInsert.Answer = answerToUpdate;
            reviewToInsert.AnswerID = aId;

            db.Reviews.Add(reviewToInsert);
            db.SaveChanges();

            List<Review> reviews = answerToUpdate.Reviews.ToList();

            int ratingCount = reviews.Count;
            int newRating = 0;

            for (int i = 0; i < ratingCount; i++)
            {
                int tempRating = reviews[i].Rating;
                newRating += tempRating;
            }

            answerToUpdate.Rating = newRating / ratingCount;

            db.SaveChanges();

            return RedirectToAction("Answer", "Review", new { aId = aId });
        }

        // GET: Review/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // GET: Review/Create
        public ActionResult Create()
        {
            ViewBag.AnswerID = new SelectList(db.Answers, "AnswerID", "FilePath");
            return MyAnswers();
        }

        // POST: Review/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReviewID,Rating,Comment,AnswerID")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AnswerID = new SelectList(db.Answers, "AnswerID", "FilePath", review.AnswerID);
            return View(review);
        }

        // GET: Review/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            ViewBag.AnswerID = new SelectList(db.Answers, "AnswerID", "FilePath", review.AnswerID);
            return View(review);
        }

        // POST: Review/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReviewID,Rating,Comment,AnswerID")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AnswerID = new SelectList(db.Answers, "AnswerID", "FilePath", review.AnswerID);
            return View(review);
        }

        // GET: Review/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
