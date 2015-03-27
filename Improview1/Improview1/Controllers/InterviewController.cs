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
using System.Windows.Media.Imaging;
using Improview1.Extensions;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
using Improview1.ServiceReference1;
using System.Threading.Tasks;

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
        public ActionResult NextPost(int iId, int qNo)
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
        public ActionResult PostRecordedAudioVideo()
        {
            foreach (string upload in Request.Files)
            {
                if (!Request.Files[upload].HasVideoFile()) continue;                // Extension method checks video has been uploaded
                                                                                    // http://www.mikesdotnetting.com/article/125/asp-net-mvc-uploading-and-downloading-files

                string fileName = "";
                string relativeServerVideoFolder = "";
                string relativeVideoPath = "";
                HttpPostedFileBase videoFile = null;

                // Upload video file to server
                try
                {
                    //string path = AppDomain.CurrentDomain.BaseDirectory + "uploads/";
                    //string path = "~/uploads/";
                    videoFile = Request.Files[upload];

                    if (videoFile == null) continue;

                    //string fP = Path.Combine(path, Request.Form[0]);
                    fileName = Request.Form[0];
                    relativeServerVideoFolder = Server.MapPath("~/uploads/");
                    relativeVideoPath = Path.Combine(relativeServerVideoFolder, fileName);

                    Session["filePath"] = relativeVideoPath;
                    videoFile.SaveAs(relativeVideoPath);

                    //Session["filePath"] = fP;
                    //file.SaveAs(fP);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                // Post video to Cloud Service set up to save it in Azure's BLOB storage
                PostVideoToCloudService(relativeVideoPath);

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

        public void PostVideoToCloudService(string filePath)
        {
            // https://social.msdn.microsoft.com/Forums/en-US/8b5d4086-f468-419a-805b-c553105d183a/how-to-convert-video-to-bytearray-in-c?forum=windowsmobiledev
            byte[] videoBytes = VideoToByteArray(filePath);
            string storageGuid = User.Identity.GetUserId();

            // create new serviceclient instance for our cloud storage service and upload our data using AddImageAsync
            if (videoBytes != null)
            {
                RunServiceCode(storageGuid, "ImproviewVideoFileUploadTest1", videoBytes);
            }
            else
            {
                Console.WriteLine("Byte array empty");
            }

            //byte[] byteArray = null;
            //System.IO.FileStream fs = new System.IO.FileStream("whatever.vmv", System.IO.FileMode.CreateNew);
            //fs.Write(byteArray, 0, byteArray.Length);
            //fs.Close();

            //Stream videoStream = Application.GetResourceStream(new Uri(@"/VideoUpload;component/test.mp4", UriKind.Relative)).Stream;

            //byte[] videoBytes = new byte[videoStream.Length];
            //videoStream.Read(videoBytes, 0, (int)videoStream.Length);
            //videoStream.Close();

            //byte[] byteArray = null;
            //// Convert the byte array to wav file
            //using (Stream stream = new MemoryStream(byteArray))
            //{
            //    // http://msdn.microsoft.com/en-us/library/ms143770%28v=VS.100%29.aspx
            //    System.Media.SoundPlayer myPlayer = new System.Media.SoundPlayer(stream);
            //    myPlayer.Play();
            //}
            //System.Media.SoundPlayer myPlayer2 = new System.Media.SoundPlayer(myfile);
            //myPlayer2.Stream = new MemoryStream();
            //myPlayer2.Play();
        }

        public byte[] VideoToByteArray(string filePath)
        {
            byte[] byteArray = null;

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            using (fs)
            {
                int chunkSize = 1024; // 1KB video chunks
                byteArray = br.ReadBytes(chunkSize);
                br.Close();
                fs.Close();
            }

            return byteArray;
        }

        public async void RunServiceCode(string storageGuid, string fileName, byte[] videoBytes)
        {
            // Increased request size in web.config to enable potentially large video files to be uploaded
            Service1Client svc = new Service1Client();
            svc.SaveFileCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(svc_SaveFileCompleted);
            await Task.Run(() => svc.SaveFileAsync(storageGuid, "ImproviewVideoFileUploadTest1", videoBytes));
            //svc.SaveImageCompleted += new EventHandler<SaveImageCompletedEventArgs>(svc_SaveImageCompleted);
            //await Task.Run(() => svc.SaveImageAsync(storageGuid, "ImproviewVideoFileUploadTest1", videoBytes));
        }

        /*public void SaveImageToBlobStorage()
        {
            // create new writeable image and set it's value to our image on screen
            WriteableBitmap bmp = new WriteableBitmap();
            
            // create new byte array and read our image soure into it;
            byte[] byteArray = new byte[4096];

            using (MemoryStream stream = new MemoryStream())
            {
                bmp.SaveJpeg(stream, bmp.PixelWidth, bmp.PixelHeight, 0, 60);
                byteArray = stream.ToArray();
            }

            // set values for uploading to cloud database for YOUR UNIQUE guid, and long/lat/description/category if applicable
            string storageGuid = User.Identity.GetUserId();
            Guid storageId = Guid.Parse(storageGuid);

            // create new serviceclient instance for our cloud storage service and upload our data using AddImageAsync
            Service1Client svc = new Service1Client();
            svc.SaveImageCompleted += new EventHandler<SaveImageCompletedEventArgs>(svc_SaveImageCompleted);
            svc.SaveImageAsync(storageGuid, "ImproviewImageUploadTest1", byteArray);
        }*/

        // code to launch when image data has been uploaded to server, can return error so handle it
        void svc_SaveFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Console.WriteLine("S'all good man!");
            }
            else
            {
                Console.WriteLine("An error occured:" + e.Error);
            }
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
