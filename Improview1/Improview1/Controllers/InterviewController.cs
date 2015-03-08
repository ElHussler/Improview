using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Improview1.DAL;
using Improview1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using Improview1.Extensions;
using System.Data.SqlClient;

namespace Improview1.Controllers
{
    public class InterviewController : Controller
    {
        private InterviewContext db = new InterviewContext();
        protected ApplicationDbContext ApplicationDbContext { get; set; }
        protected UserManager<User> UserManager { get; set; }

        public InterviewController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<User>(new UserStore<User>(this.ApplicationDbContext));
        }

        // GET: Interview
        public ActionResult Index()
        {
            return View(db.Interviews.ToList());
        }

        // GET: Interview/Next/1
        public ActionResult Next(int iId, int qNo)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.interviewId = iId;
                ViewBag.questionNum = qNo;

                Interview interview = db.Interviews.Find(iId);
                Question question = interview.Questions.ToList()[qNo];

                ViewBag.totalQuestions = interview.Questions.Count();

                return View(question);
            }
            else
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Interview/Index" });
            }
        }

        // POST
        [HttpPost]
        public ActionResult NextPost(int iId, int qNo)//, string anT)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.interviewId = iId;
                Interview interview = db.Interviews.Find(iId);

                string filePath = Session["filePath"].ToString();

                Answer answer = new Answer();
                answer.Number = qNo;
                answer.FilePath = filePath;
                answer.IsRecorded = (filePath.EndsWith("webm")) ? true : false;
                answer.UserID = User.Identity.GetUserId();
                answer.Interview = interview;

                Session.Remove("filePath");

                db.Answers.Add(answer);
                db.SaveChanges();

                if (qNo < interview.Questions.Count)
                {
                    Question question = interview.Questions.ToList()[qNo];

                    ViewBag.totalQuestions = interview.Questions.Count();

                    return View("Next", question);
                }
                else
                {
                    return RedirectToAction("Finish", new { iId = ViewBag.interviewId });
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult PostRecordedAudioVideo()
        {
            foreach (string upload in Request.Files)
            {
                if (!Request.Files[upload].HasVideoFile()) continue;                // Extension method checks video has been uploaded
                                                                                    // http://www.mikesdotnetting.com/article/125/asp-net-mvc-uploading-and-downloading-files
                //string path = AppDomain.CurrentDomain.BaseDirectory + "uploads/";
                string path = "~/uploads/";
                HttpPostedFileBase file = Request.Files[upload];
                if (file == null) continue;

                string fP = Path.Combine(path, Request.Form[0]);

                file.SaveAs(fP);

                Session["filePath"] = fP;

                // Sample code for saving posted image to database, not using if storing on server?
                /*string mimeType = Request.Files[upload].ContentType;
                Stream fileStream = Request.Files[upload].InputStream;
                string fileName = Path.GetFileName(Request.Files[upload].FileName);
                int fileLength = Request.Files[upload].ContentLength;
                byte[] fileData = new byte[fileLength];
                fileStream.Read(fileData, 0, fileLength);

                const string connect = @"Server=.\SQLExpress;Database=FileTest;Trusted_Connection=True;";
                using (var conn = new SqlConnection(connect))
                {
                    var qry = "INSERT INTO FileStore (FileContent, MimeType, FileName) VALUES (@FileContent, @MimeType, @FileName)";
                    var cmd = new SqlCommand(qry, conn);
                    cmd.Parameters.AddWithValue("@FileContent", fileData);
                    cmd.Parameters.AddWithValue("@MimeType", mimeType);
                    cmd.Parameters.AddWithValue("@FileName", fileName);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }*/
            }

            return Json(Request.Form[0]);
        }

        [HttpPost]
        public ActionResult DeleteFile()
        {
            var fileUrl = AppDomain.CurrentDomain.BaseDirectory + "uploads/" + Request.Form["delete-file"];
            new FileInfo(fileUrl + ".wav").Delete();
            new FileInfo(fileUrl + ".webm").Delete();
            return Json(true);
        }

        public ActionResult Finish(int iId)
        {
            if (User.Identity.IsAuthenticated)
            {
                Interview interview = db.Interviews.Find(iId);
                return View(interview);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        // GET: Interview/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Interview interview = db.Interviews.Find(id);
            if (interview == null)
            {
                return HttpNotFound();
            }
            return View(interview);
        }

        // GET: Interview/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Interview/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "InterviewID")] Interview interview)
        {
            if (ModelState.IsValid)
            {
                db.Interviews.Add(interview);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(interview);
        }

        // GET: Interview/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Interview interview = db.Interviews.Find(id);
            if (interview == null)
            {
                return HttpNotFound();
            }
            return View(interview);
        }

        // POST: Interview/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InterviewID")] Interview interview)
        {
            if (ModelState.IsValid)
            {
                db.Entry(interview).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(interview);
        }

        // GET: Interview/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Interview interview = db.Interviews.Find(id);
            if (interview == null)
            {
                return HttpNotFound();
            }
            return View(interview);
        }

        // POST: Interview/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Interview interview = db.Interviews.Find(id);
            db.Interviews.Remove(interview);
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
