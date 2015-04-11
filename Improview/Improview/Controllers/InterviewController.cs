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
using System.IO;
//using System.Windows.Media.Imaging;
using Improview.Extensions;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
//using Improview.ServiceReference1;
using System.Threading.Tasks;
using Improview.ServiceReference1;

namespace Improview.Controllers
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
        public ActionResult NextPost(int iId, int qNo)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.interviewId = iId;
                Interview interview = db.Interviews.Find(iId);

                string filePathRelative = Session["filePathRelative"].ToString();
                string filePathAbsolute = Session["filePathAbsolute"].ToString();
                string filePathAzure = Session["filePathAzure"].ToString();

                Answer answer = new Answer();
                answer.Number = qNo;
                answer.FilePathServerRelative = filePathRelative;
                answer.FilePathServerAbsolute = filePathAbsolute;
                answer.FilePathAzureBlobStorage = filePathAzure;
                answer.IsRecorded = (filePathRelative.EndsWith("webm")) ? true : false;
                answer.UserID = User.Identity.GetUserId();
                answer.Interview = interview;

                Session.Remove("filePathRelative");
                Session.Remove("filePathAbsolute");
                Session.Remove("filePathAzure");

                try
                {
                    db.Answers.Add(answer);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }

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
        public async Task PostRecordedAudioVideo()
        {
            ViewBag.isVideoPosted = false;
            foreach (string upload in Request.Files)
            {
                if (!Request.Files[upload].HasVideoFile()) continue;                // Extension method checks video has been uploaded
                                                                                    // http://www.mikesdotnetting.com/article/125/asp-net-mvc-uploading-and-downloading-files
                string videoFileName = "";
                string absoluteServerVideoFolder = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";
                string relativeServerVideoFolder = Server.MapPath("~/Uploads/");
                string absoluteVideoPath = "";
                string relativeVideoPath = "";
                HttpPostedFileBase videoFile = null;

                // Upload video file to server
                try
                {
                    videoFile = Request.Files[upload];
                    if (videoFile == null) continue;

                    videoFileName = Request.Form[0];
                    
                    relativeVideoPath = Path.Combine(relativeServerVideoFolder, videoFileName);
                    absoluteVideoPath = Path.Combine(absoluteServerVideoFolder, videoFileName);

                    Session["filePathRelative"] = relativeVideoPath;
                    Session["filePathAbsolute"] = absoluteVideoPath;

                    videoFile.SaveAs(absoluteVideoPath);

                    //Session["filePath"] = fP;
                    //file.SaveAs(fP);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                try
                {
                    // Post video to Cloud Service set up to save it in Azure's BLOB storage
                    await PostVideoToCloudService(relativeVideoPath, videoFileName);
                    ViewBag.isVideoPosted = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public async Task PostVideoToCloudService(string filePath, string fileName)
        {
            // https://social.msdn.microsoft.com/Forums/en-US/8b5d4086-f468-419a-805b-c553105d183a/how-to-convert-video-to-bytearray-in-c?forum=windowsmobiledev
            byte[] videoBytes = VideoToByteArray(filePath);
            string storageGuid = User.Identity.GetUserId();

            if (videoBytes != null)
            {
                // Increased request size in web.config to enable potentially large video file uploads
                Service1Client svc = new Service1Client();
                svc.SaveFileCompleted += new EventHandler<SaveFileCompletedEventArgs>(svc_SaveFileCompleted);
                // Get video BLOB file path returned by running 'SaveFile' (on a separate thread)
                string filePathAzure = await Task.Run(() => svc.SaveFile(storageGuid, fileName, videoBytes));
                Session["filePathAzure"] = filePathAzure;
            }
            else
            {
                Console.WriteLine("Byte array empty");
            }
        }

        public byte[] VideoToByteArray(string filePath)
        {
            byte[] byteArray = null;

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            try
            {
                byteArray = br.ReadAllBytes();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }

            br.Close();
            
            return byteArray;
        }

        public ActionResult Finish(int iId)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Title = "Complete!";
                Interview interview = db.Interviews.Find(iId);
                return View(interview);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        void svc_SaveFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Console.WriteLine("Save Complete");
            }
            else
            {
                Console.WriteLine("An error occured:" + e.Error);
            }
        }

        [HttpPost]
        public ActionResult DeleteFile()
        {
            var fileUrl = AppDomain.CurrentDomain.BaseDirectory + "Uploads/" + Request.Form["delete-file"];
            new FileInfo(fileUrl + ".wav").Delete();
            new FileInfo(fileUrl + ".webm").Delete();
            return Json(true);
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
