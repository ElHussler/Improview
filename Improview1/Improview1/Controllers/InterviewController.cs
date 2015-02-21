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
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public InterviewController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        // GET: Interview
        public ActionResult Index()
        {
            return View(db.Interviews.ToList());
        }

        // GET: Interview/Next/1
        public ActionResult Next(int interviewId, int questionNum)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.interviewId = interviewId;
                ViewBag.questionNum = questionNum;

                Interview interview = db.Interviews.Find(ViewBag.interviewId);
                Question question = interview.Questions.ToList()[questionNum];

                return View(question);
            }
            else
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Interview/Index" });
            }
        }

        [HttpParamAction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Record()
        {
            return View();
        }

        [HttpParamAction]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Stop(int iId, int qNo, string fP)
        {
            return RedirectToAction("Next", new { iId = iId, qNo = qNo, fP = fP });
        }

        // POST
        [HttpPost]
        public ActionResult Next(int iId, int qNo, string fP)//, string anT)
        {
            if (User.Identity.IsAuthenticated)
            {
                string filePath = fP;
                ViewBag.interviewId = iId;
                Interview interview = db.Interviews.Find(ViewBag.interviewId);

                Answer answer = new Answer();
                answer.Number = qNo;
                answer.Text = fP;//"";//anT;
                //answer.FilePath = filePath;
                answer.IsRecorded = false;
                answer.UserID = User.Identity.GetUserId();
                answer.Interview = interview;

                db.Answers.Add(answer);
                db.SaveChanges();

                if (qNo < interview.Questions.Count)
                {
                    Question question = interview.Questions.ToList()[qNo];
                    return View(question);
                }
                else
                {
                    return RedirectToAction("Finish", new { interviewId = ViewBag.interviewId });
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
            var filePath = "";

            foreach (string upload in Request.Files)
            {
                if (!Request.Files[upload].HasVideoFile()) continue;                // Extension method checks video has been uploaded
                                                                                    // http://www.mikesdotnetting.com/article/125/asp-net-mvc-uploading-and-downloading-files
                string path = AppDomain.CurrentDomain.BaseDirectory + "uploads/";
                HttpPostedFileBase file = Request.Files[upload];
                if (file == null) continue;

                filePath = Path.Combine(path, Request.Form[0]);

                file.SaveAs(filePath);

                // Saving posted image(s) to database
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

            ViewBag.filePath = filePath;

            return RedirectToAction("Next", new { iId = ViewBag.interviewId, qNo = ViewBag.questionNum, fP = filePath });
            //return Json(filePath);
            //return View();
        }

        [HttpPost]
        public ActionResult DeleteFile()
        {
            var fileUrl = AppDomain.CurrentDomain.BaseDirectory + "uploads/" + Request.Form["delete-file"];
            new FileInfo(fileUrl + ".wav").Delete();
            new FileInfo(fileUrl + ".webm").Delete();
            return Json(true);
        }

        public ActionResult Finish(int interviewId)
        {
            if (User.Identity.IsAuthenticated)
            {
                Interview interview = db.Interviews.Find(interviewId);
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
