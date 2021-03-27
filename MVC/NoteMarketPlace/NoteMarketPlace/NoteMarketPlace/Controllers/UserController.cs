using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NoteMarketPlace.Models;
using System.Net.Mail;
using System.Net;
using System.Web.Security;
using System.IO;
using System.Data;
using System.Windows;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading.Tasks;



namespace NoteMarketPlace.Controllers
{
    public class UserController : Controller
    {
        NoteMarketPlaceDatabaseEntities db = new NoteMarketPlaceDatabaseEntities();
        // GET: User Login Page 
        [HttpGet]
        public ActionResult Index()
        {
            HttpCookie cookie = Request.Cookies["remember"];
            if (cookie != null)
            {
                ViewBag.email = cookie["email"].ToString();
                ViewBag.password = cookie["password"].ToString();
               // ViewBag.id = Convert.ToInt32(cookie["id"]);
            }
            return View();
        }


        // POST: User Login Page 
        [HttpPost]
        public ActionResult Index(User u2,bool RememberMe)
        {
                var userDetails = db.Users.Where(m => m.EmailID.Equals(u2.EmailID) && m.Password.Equals(u2.Password)).FirstOrDefault();
               if (userDetails == null)
               {
                    u2.LoginErrorMessage = "The password that you have entered is incorrect.";
                    return View("Index", u2);
               }
               else
               {
                    
                    Session["UserEmailId"] = u2.EmailID.ToString();

                    var v = db.Users.FirstOrDefault(m => m.EmailID == u2.EmailID);
                    if (v != null)
                    {
                        Session["UserId"] = v.ID.ToString();
                    }

                    //exampleCheck1
                    HttpCookie cookie = new HttpCookie("remember");
                    if (u2.RememberMe == true)
                    {

                        cookie["email"] = u2.EmailID;
                        cookie["password"] = u2.Password;
                        //cookie["id"] = u2.ID.ToString();
                        cookie.Expires = DateTime.Now.AddMonths(1);
                        HttpContext.Response.Cookies.Add(cookie);
                        //getCookie = Request.Cookies["User"].Value;
                    }
                    else
                    {
                        cookie.Expires = DateTime.Now.AddMonths(-1);
                        HttpContext.Response.Cookies.Add(cookie);
                    }

                }

               if(userDetails.RoleID == 1)
               {
                    var updateProfile = db.Profiles.FirstOrDefault(m => m.SellerID == userDetails.ID);
                    if(updateProfile == null)
                    {
                        return RedirectToAction("UserProfile", "User");
                    }
                    else
                    {
                        return RedirectToAction("SearchNotes", "User");
                    }
                    
               }
                else
                {
                    return RedirectToAction("AdminDashboard", "Admin");
                }

            
            //return RedirectToAction("SearchNotes", "User");

        }
        
        // GET: User Logout Page 
        public ActionResult logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "User");
        }


        // GET: User SignUp Page 
        public ActionResult SignUp()
        {
            return View();
        }


        // POST: User SignUp Page 
        [HttpPost]
        public ActionResult SignUp(User u1)
        {
            string message = "";
            //Model Validaiton
            if (ModelState.IsValid)
            {
                //Email is already exist
                var isExist = IsEmailExist(u1.EmailID);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View("SignUp");
                }
                


                u1.RoleID = 1;
                u1.CreatedDate = DateTime.Now;
                
                u1.IsActive = true;

                db.Users.Add(u1);
                db.SaveChanges();

                int a = db.SaveChanges();
                if (a>0)
                {
                    ViewBag.success_message = "Your account has been successfully created.";
                }
                else
                {
                    ViewBag.success_message = "Your account not created.";
                }

                //send email to user
                sendVerificationLinkEmail(u1.EmailID, u1.FirstName);
                //message = "Registration successfully done;

                return View("SignUp");
            }
            else
            {
                message = "Invalid Request";
            }

            return View();
        }


        // GET: User EmailVerificaiton Page 
        public ActionResult EmailVerification()
        {
            return View();
        }


        // POST: User EmailVerificaiton Page
        [HttpGet]
        public ActionResult EmailVerification(string emailID)
        {
            db.Configuration.ValidateOnSaveEnabled = false;

            var v = db.Users.FirstOrDefault(m => m.EmailID == emailID);
            if (v != null)
            {
                v.IsEmailVerified = true;
                ViewBag.FirstName = v.FirstName;
                db.SaveChanges();
            }
            
            
            return View();
        }


        // GET: User Forgetpassword Page
        public ActionResult ForgetPassword()
        {
            return View();
        }


        // POST: User Forgetpassword Page
        [HttpPost]
        public ActionResult ForgetPassword(User u3)
        {
            //Verify Email Id
            //Generate Reset password Link
            //Send email

            //string message = "i";
            //bool status = false;

            var account = db.Users.Where(m => m.EmailID == u3.EmailID).FirstOrDefault();
            if (account != null)
            {
                //Send email for reset password
                // return View("ContactUs");
                sendEmail(u3.EmailID, u3.Password);
            }

            return View();
        }


        // GET: User ContactUS Page
        public ActionResult ContactUs()
        {
               return View();
            
        }

        // POST: User ContactUS Page
        [HttpPost]
        public ActionResult ContactUs(string Name, string EmailAddress, string Subject, string Comment)
        {
            //Error message Display
            
                var fromEmail = new MailAddress("3999vachauhan@gmail.com", "V@chauhan3999");
                var toEmail = new MailAddress("pateldhara1019@gmail.com", "Dhara1112");
                var fromEmailPassword = "nnumqfegkqbdecgs";

                string subject = Subject + "- Query";

                string body = "Hello, <br/>" + "<br/>" + Comment + "<br/><br/> Regards," + "<br/>" + Name + "<br/>" + EmailAddress;

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
                };

                using (var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                    smtp.Send(message);
                 return RedirectToAction("Index","User");
            
        }

        // GET: User FAQ Page
        public ActionResult FAQ()
        {
            return View();
        }


        // GET: User BuyersRequest Page , string SortOrder, string SortBy, int PageNumber = 1
        
        public ActionResult BuyersRequest(int? bookId, int? buyerId,int? downloadId, string SortOrder, string SortBy, int PageNumber = 1)
        {

            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("Index", "User");
            }

            else
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                var q3 = db.Notes.FirstOrDefault(m => m.NoteID == bookId);
                //var v = db.DownloadedNotes.FirstOrDefault(m => m.BuyersID == buyerId);

                var v = db.DownloadedNotes.FirstOrDefault(m => m.ID == downloadId);
                
                
                if(v != null)
                {
                    //if(v.IsAttachmentDownloaded == false)
                    //{
                    //    v.IsSellerHasAllowedDownload = true;
                    //    db.Entry(v).State = System.Data.Entity.EntityState.Modified;

                    //    v.AttachmentPath = q3.NoteAttachment;
                    //    db.Entry(v).State = System.Data.Entity.EntityState.Modified;

                    //    db.SaveChanges();

                    //}
                    //else
                    //{
                    //    ViewBag.e = "No";
                    //}

                    v.IsSellerHasAllowedDownload = true;
                    db.Entry(v).State = System.Data.Entity.EntityState.Modified;

                    v.AttachmentPath = q3.NoteAttachment;
                    db.Entry(v).State = System.Data.Entity.EntityState.Modified;

                    v.IsPaid = true;
                    db.Entry(v).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    //ViewBag.ei = "yes";

                }

                else
                {
                    //ViewBag.ei = "No";
                }


                List<Note> noteValues = db.Notes.ToList();
                //List<NoteCountry> countryValues = db.NoteCountries.ToList();
                List<User> userValues = db.Users.ToList();
                List<DownloadedNote> downloadValues = db.DownloadedNotes.ToList();
                List<ReferenceData> referenceDataValues = db.ReferenceDatas.ToList();
                List<NoteCategory> categoryValues = db.NoteCategories.ToList();

                var sID = Convert.ToInt32(Session["UserId"]);

                var query1 = from u in userValues
                             join d in downloadValues on u.ID equals d.BuyersID
                             join n in noteValues on d.NoteID equals n.NoteID
                             join nc in categoryValues on n.Category equals nc.ID
                             join rd in referenceDataValues on n.SellFor equals rd.ID
                             where n.SellerID == sID && d.IsSellerHasAllowedDownload == false
                             select new NoteDetailsViewModel { note = n, user = u, download = d, noteCategory = nc, referenceData = rd };
 

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;

                query1 = ApplySorting(SortOrder, SortBy, query1);
                query1 = ApplyPagination(query1, PageNumber);

                return View(query1);

            }

        }

        [HttpPost]
        public async Task<ActionResult> BuyersRequest(string searching, string SortOrder, string SortBy, int PageNumber = 1) 
        {
            ViewData["GetBuyersDetails"] = searching;

            
            List<Note> noteValues = db.Notes.ToList();
            //List<NoteCountry> countryValues = db.NoteCountries.ToList();
            List<User> userValues = db.Users.ToList();
            List<DownloadedNote> downloadValues = db.DownloadedNotes.ToList();
            List<ReferenceData> referenceDataValues = db.ReferenceDatas.ToList();
            List<NoteCategory> categoryValues = db.NoteCategories.ToList();

            var sID = Convert.ToInt32(Session["UserId"]);

            var query1 = from u in userValues
                         join d in downloadValues on u.ID equals d.BuyersID
                         join n in noteValues on d.NoteID equals n.NoteID
                         join nc in categoryValues on n.Category equals nc.ID
                         join rd in referenceDataValues on n.SellFor equals rd.ID
                         where n.SellerID == sID
                         select new NoteDetailsViewModel { note = n, user = u, download = d, noteCategory = nc, referenceData = rd };
           

            if (!string.IsNullOrEmpty(searching))
            {
                query1 = query1.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.NoteTitle.Contains(searching) 
                || NoteDetailsViewModel.note.CourseName.Contains(searching));
                query1 = ApplySorting(SortOrder, SortBy, query1);
                query1 = ApplyPagination(query1, PageNumber);
            }

            return View("BuyersRequest", query1);
        }

        [HttpGet]
        // GET: User Search note Page
        public ActionResult SearchNotes(string textbox,int? typeOfNote,int? categoryOfNote,string instituteOfNote, string courseOfNote,int? countryNote, string rateOfNote, int PageNumber = 1)
        {
            /*if (Session["UserEmailId"] == null)
            {

                return RedirectToAction("Index", "User");
            }*/

            //else
            //{
            //string univer[] =  ;
            if(typeOfNote == null)
            {
                ViewBag.search = "no";
            }
            else
            {
                ViewBag.search = instituteOfNote;
            }
           

                //Type
                var typeList = db.NoteTypes.ToList();
                ViewBag.TypeList = new SelectList(typeList, "ID", "TypeName");

                //Category
                var categoryList = db.NoteCategories.ToList();
                ViewBag.CategoryList = new SelectList(categoryList, "ID", "CategoryName");

                //University
                //var universityList = db.Notes.ToList();
                //ViewBag.UniversityList = new SelectList(universityList, "Institute", "Institute");

                ViewBag.UniversityList = db.Notes.Where(m => m.Institute != null).Select(m => m.Institute).Distinct();

                //Course
                //var courseList = db.Notes.ToList();
                //    var courseList1 = courseList.Distinct().ToList();
                //    ViewBag.CourseList = new SelectList(courseList, "CourseName", "CourseName");
                ViewBag.CourseList = db.Notes.Where(m => m.CourseName != null).Select(m => m.CourseName).Distinct();


                //Country
                var countryList = db.NoteCountries.ToList();
                ViewBag.CountyList = new SelectList(countryList, "ID", "CountryName");

            //Rating
            //var ratingList = db.NoteReviews.ToList();
            //ViewBag.RatingList = new SelectList(ratingList, "Ratings", "Ratings");
            ViewBag.RatingList = db.NoteReviews.Where(m => m.Ratings != 0).Select(m => m.Ratings).Distinct();

                List<Note> noteValues = db.Notes.ToList();
                List<NoteCountry> countryValues = db.NoteCountries.ToList();
                List<NoteCategory> categoryValuse = db.NoteCategories.ToList();
                List<NoteType> typeValues = db.NoteTypes.ToList();
                List<NoteReview> reviewValues = db.NoteReviews.ToList();

                
                 var NotejoinCountry = from n in noteValues
                                        join cv in countryValues on n.Country equals cv.ID
                                        join nc in categoryValuse on n.Category equals nc.ID
                                        join nt in typeValues on n.NoteType equals nt.ID
                                        //join nr in reviewValues on n.NoteID equals nr.NoteID
                                        //where n.NoteTitle == "cp"
                                        select new NoteDetailsViewModel { countryDetails = cv, noteDetails = n, categoryDetails = nc, typeDetails = nt/*, noteReview = nr*/ };

            if (!string.IsNullOrEmpty(textbox))
            {
                
                NotejoinCountry = NotejoinCountry.Where(NoteDetailsViewModel => NoteDetailsViewModel.noteDetails.NoteTitle.ToLower().StartsWith(textbox.ToLower())
                                  || NoteDetailsViewModel.categoryDetails.CategoryName.ToLower().StartsWith(textbox.ToLower())
                                  //|| NoteDetailsViewModel.typeDetails.ID.Equals(typeOfNote)
                                  //|| NoteDetailsViewModel.categoryDetails.ID.Equals(categoryOfNote)
                                  //|| NoteDetailsViewModel.noteDetails.Institute.ToLower().StartsWith(instituteOfNote.ToLower())
                                  //|| NoteDetailsViewModel.noteDetails.CourseName.ToLower().StartsWith(courseOfNote.ToLower())
                                  );
            }

            if(typeOfNote != null)
            {
                NotejoinCountry = NotejoinCountry.Where(NoteDetailsViewModel => NoteDetailsViewModel.typeDetails.ID.Equals(typeOfNote));
            }

            if (categoryOfNote != null)
            {
                NotejoinCountry = NotejoinCountry.Where(NoteDetailsViewModel => NoteDetailsViewModel.categoryDetails.ID.Equals(categoryOfNote));
            }

            if (!string.IsNullOrEmpty(instituteOfNote))
            {
                NotejoinCountry = NotejoinCountry.Where(NoteDetailsViewModel => NoteDetailsViewModel.noteDetails.Institute.ToLower().StartsWith(instituteOfNote.ToLower()));
            }

            if (!string.IsNullOrEmpty(courseOfNote))
            {
                NotejoinCountry = NotejoinCountry.Where(NoteDetailsViewModel => NoteDetailsViewModel.noteDetails.CourseName.ToLower().StartsWith(courseOfNote.ToLower()));
            }

            if (countryNote != null)
            {
                NotejoinCountry = NotejoinCountry.Where(NoteDetailsViewModel => NoteDetailsViewModel.countryDetails.ID.Equals(countryNote));
            }

            if (!string.IsNullOrEmpty(rateOfNote))
            {
               // NotejoinCountry = NotejoinCountry.Where(NoteDetailsViewModel => NoteDetailsViewModel.noteReview.Ratings.Equals(rateOfNote));
                NotejoinCountry = from n in noteValues
                                  join cv in countryValues on n.Country equals cv.ID
                                  join nc in categoryValuse on n.Category equals nc.ID
                                  join nt in typeValues on n.NoteType equals nt.ID
                                  join nr in reviewValues on n.NoteID equals nr.NoteID
                                  where nr.Ratings == Convert.ToInt32(rateOfNote)
                                  select new NoteDetailsViewModel { countryDetails = cv, noteDetails = n, categoryDetails = nc, typeDetails = nt, noteReview = nr };
            }

            //ViewBag.SortOrder = SortOrder countryNote rateOfNote;
            //ViewBag.SortBy = SortBy;
            //NotejoinCountry = ApplySorting(SortOrder, SortBy, NotejoinCountry);
            NotejoinCountry = ApplyPagination(NotejoinCountry, PageNumber);


            return View(NotejoinCountry);
                //return View();
            //}

            //return View(from Note in db.Notes select Note);
        }

        

       [HttpPost]
        public ActionResult SearchNotes(string inputgender, int PageNumber = 1)
        {
            ViewBag.uni = inputgender;

            //Type
            var typeList = db.NoteTypes.ToList();
            ViewBag.TypeList = new SelectList(typeList, "ID", "TypeName");

            //Category
            var categoryList = db.NoteCategories.ToList();
            ViewBag.CategoryList = new SelectList(categoryList, "ID", "CategoryName");

            //University
            var universityList = db.Notes.ToList();
            ViewBag.UniversityList = new SelectList(universityList, "Institute", "Institute");

            //Course
            var courseList = db.Notes.ToList();
            var courseList1 = courseList.Distinct().ToList();
            ViewBag.CourseList = new SelectList(courseList, "CourseName", "CourseName");

            //Country
            var countryList = db.NoteCountries.ToList();
            ViewBag.CountyList = new SelectList(countryList, "ID", "CountryName");

            //Rating
            var ratingList = db.NoteReviews.ToList();
            ViewBag.RatingList = new SelectList(ratingList, "Ratings", "Ratings");

            List<Note> noteValues = db.Notes.ToList();
            List<NoteCountry> countryValues = db.NoteCountries.ToList();
            List<NoteCategory> categoryValuse = db.NoteCategories.ToList();
            List<NoteType> typeValues = db.NoteTypes.ToList();


            var NotejoinCountry = from n in noteValues
                                  join cv in countryValues on n.Country equals cv.ID
                                  join nc in categoryValuse on n.Category equals nc.ID
                                  join nt in typeValues on n.NoteType equals nt.ID
                                  //where n.NoteTitle == "cp"
                                  select new NoteDetailsViewModel { countryDetails = cv, noteDetails = n, categoryDetails = nc, typeDetails = nt };

            //NotejoinCountry = ApplySorting(SortOrder, SortBy, NotejoinCountry);
            NotejoinCountry = ApplyPagination(NotejoinCountry, PageNumber);

            return View(NotejoinCountry);
        }

        

        // GET: User Addnote Page
        public ActionResult AddNotes(int? nId)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Index", "User");
            }

            var categoryList = db.NoteCategories.ToList();
            ViewBag.CategoryList = new SelectList(categoryList,"ID","CategoryName");
            //ViewBag.CategoryList = categoryList;
            //Note n2 = new Note();

            //n2.CategoryList = new SelectList(categoryList, "ID", "CategoryName");


            var countryList = db.NoteCountries.ToList();
            ViewBag.CountyList = new SelectList(countryList, "ID", "CountryName");

            var typeList = db.NoteTypes.ToList();
            ViewBag.TypeList = new SelectList(typeList, "ID", "TypeName");

            if(nId == null)
            {
                return View();
            }
            else
            {
                var temp = db.Notes.FirstOrDefault(m => m.NoteID == nId);
                
                return View(temp);
            }

            
        }

        // POST: User Addnote Page
        [HttpPost]
        public ActionResult AddNotes(int? nId, Note n1, string save, string published)
        {

            
            if (nId == null)
                {

                n1.SellerID = Convert.ToInt32(Session["UserId"]);
                n1.NoteSize = 15;

                if (n1.DisplayPicture == null)
                {
                    n1.DisplayPicture = "~/Image/search1215941230.png";
                }
                else
                {
                    //Image upload for book image
                    string filename = Path.GetFileNameWithoutExtension(n1.BookImage.FileName);
                    string extension = Path.GetExtension(n1.BookImage.FileName);

                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;

                    n1.DisplayPicture = "~/Image/" + filename;

                    filename = Path.Combine(Server.MapPath("~/Image/"), filename);

                    n1.BookImage.SaveAs(filename);
                }


                //Pdf of note upload path in databse
                string noteName = Path.GetFileNameWithoutExtension(n1.NotePdf.FileName);
                string noteExtension = Path.GetExtension(n1.NotePdf.FileName);

                noteName = noteName + DateTime.Now.ToString("yymmssfff") + noteExtension;

                n1.NoteAttachment = "~/PdfNotes/" + noteName;

                noteName = Path.Combine(Server.MapPath("~/PdfNotes/"), noteName);

                n1.NotePdf.SaveAs(noteName);

                //Note Preview upload preview path in database
                string notePreviewName = Path.GetFileNameWithoutExtension(n1.PreviewOfNote.FileName);
                string notePreviewExtension = Path.GetExtension(n1.PreviewOfNote.FileName);

                notePreviewName = notePreviewName + DateTime.Now.ToString("yymmssfff") + notePreviewExtension;

                n1.NotePreview = "~/PreviewOfNotes/" + notePreviewName;

                notePreviewName = Path.Combine(Server.MapPath("~/PreviewOfNotes/"), notePreviewName);

                n1.NotePdf.SaveAs(notePreviewName);

                //Status of note
                if (save != null)
                {
                    n1.Status = 6;
                }

                if (!string.IsNullOrEmpty(published))
                {
                    n1.Status = 9;
                }

                //Database Save
                n1.IsActive = true;
                n1.PublishedDate = DateTime.Now;

                db.DownloadedNotes = null;
                db.Users = null;
                db.NoteCategories = null;
                db.NoteCountries = null;
                db.NoteTypes = null;
                db.ReferenceDatas = null;

                db.Notes.Add(n1);

                //db.Entry(n1).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                var temp = db.Notes.FirstOrDefault(m => m.NoteID == nId);
                    if(temp != null)
                    {
                        temp.SellerID = Convert.ToInt32(Session["UserId"]);
                        temp.NoteSize = 15;

                        if (n1.DisplayPicture == null)
                        {
                            temp.DisplayPicture = "~/Image/search1215941230.png";
                        }
                        else
                        {
                            //Image upload for book image
                            string filename = Path.GetFileNameWithoutExtension(n1.BookImage.FileName);
                            string extension = Path.GetExtension(n1.BookImage.FileName);

                            filename = filename + DateTime.Now.ToString("yymmssfff") + extension;

                            temp.DisplayPicture = "~/Image/" + filename;

                            filename = Path.Combine(Server.MapPath("~/Image/"), filename);

                            temp.BookImage.SaveAs(filename);
                        }


                        //Pdf of note upload path in databse
                        string noteName = Path.GetFileNameWithoutExtension(n1.NotePdf.FileName);
                        string noteExtension = Path.GetExtension(n1.NotePdf.FileName);

                        noteName = noteName + DateTime.Now.ToString("yymmssfff") + noteExtension;

                        temp.NoteAttachment = "~/PdfNotes/" + noteName;

                        noteName = Path.Combine(Server.MapPath("~/PdfNotes/"), noteName);

                        n1.NotePdf.SaveAs(noteName);

                        //Note Preview upload preview path in database
                        string notePreviewName = Path.GetFileNameWithoutExtension(n1.PreviewOfNote.FileName);
                        string notePreviewExtension = Path.GetExtension(n1.PreviewOfNote.FileName);

                        notePreviewName = notePreviewName + DateTime.Now.ToString("yymmssfff") + notePreviewExtension;

                        temp.NotePreview = "~/PreviewOfNotes/" + notePreviewName;

                        notePreviewName = Path.Combine(Server.MapPath("~/PreviewOfNotes/"), notePreviewName);

                        n1.NotePdf.SaveAs(notePreviewName);

                        //Status of note
                        if (save != null)
                        {
                            temp.Status = 6;
                        }

                        if (!string.IsNullOrEmpty(published))
                        {
                            temp.Status = 9;
                        }

                        //Database Save
                        //Extra
                        temp.NoteTitle = n1.NoteTitle;
                        temp.Category = n1.Category;
                        temp.NoteType = n1.NoteType;
                        temp.Pages = n1.Pages;
                        temp.NoteDescription = n1.NoteDescription;
                        temp.Country = n1.Country;
                        temp.Institute = n1.Institute;
                        temp.CourseName = n1.CourseName;
                        temp.CourseCode = n1.CourseCode;
                        temp.Professor = n1.Professor;
                        temp.SellFor = n1.SellFor;
                        temp.SellPrice = n1.SellPrice;
                        temp.IsActive = true;
                        temp.PublishedDate = DateTime.Now;

                        db.DownloadedNotes = null;
                        db.Users = null;
                        db.NoteCategories = null;
                        db.NoteCountries = null;
                        db.NoteTypes = null;
                        db.ReferenceDatas = null;

                        db.Entry(temp).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        //db.Notes.Add(temp);

                        //db.Entry(n1).State = System.Data.Entity.EntityState.Modified;
                    }
            }


                db.SaveChanges();
            
                return RedirectToAction("ContactUs");

        }

        

       public ActionResult NoteDetails(string noteName)
        {

            if (Session["UserEmailId"] == null)
            {

                //return RedirectToAction("Index", "User");
                return View();
            }

            //ViewBag.name = noteName;
            else
            {
                var bMail = Session["UserEmailId"].ToString();
                var b1 = db.Users.FirstOrDefault(m => m.EmailID == bMail);
                if (b1 != null)
                {
                    ViewBag.BuyerName = b1.FirstName;
                }

                var testNote = db.Notes.FirstOrDefault(m => m.NoteTitle == noteName);
                //var testNoteReview = db.NoteReviews.Where(m => m.NoteID == testNote.NoteID);
                if (testNote != null)
                {
                    ViewBag.noteID = testNote.NoteID;
                    ViewBag.nid = testNote.SellerID;

                    NoteDetailsViewModel model = new NoteDetailsViewModel
                    {
                        note = testNote,
                        country = db.NoteCountries.SingleOrDefault(c => c.ID == testNote.Country),
                        user = db.Users.SingleOrDefault(c => c.ID == testNote.SellerID),
                        //noteReview = db.NoteReviews.SingleOrDefault(c => c.NoteID == testNote.NoteID),
                        //profileUser = db.Profiles.SingleOrDefault(c => c.SellerID == testNote.SellerID)
                        
                    };
                   

                    return View(model);
                }

                else
                {
                    return View();
                }

            }
        }

        

        [ChildActionOnly]
        public ActionResult _InfoNoteDetails(string noteName)
        {
            var testNote = db.Notes.FirstOrDefault(m => m.NoteTitle == noteName);
            //var testNoteReview = db.NoteReviews.Where(m => m.NoteID == testNote.NoteID);
            if (testNote != null)
            {
                ViewBag.noteID = testNote.NoteID;
                ViewBag.nid = testNote.SellerID;
                


                NoteDetailsViewModel model = new NoteDetailsViewModel
                {
                    note = testNote,
                    country = db.NoteCountries.SingleOrDefault(c => c.ID == testNote.Country),
                    user = db.Users.SingleOrDefault(c => c.ID == testNote.SellerID),
                    //noteReview = db.NoteReviews.SingleOrDefault(c => c.NoteID == testNote.NoteID),
                    //profileUser = db.Profiles.SingleOrDefault(c => c.SellerID == testNote.SellerID)

                };

                return PartialView(model);
                
            }

            return PartialView();


        }

        [ChildActionOnly]
        public ActionResult _InfoNoteReview(string noteName)
        {
            var testNote = db.Notes.FirstOrDefault(m => m.NoteTitle == noteName);
            //var testNoteReview = db.NoteReviews.Where(m => m.NoteID == testNote.NoteID);
            if (testNote != null)
            {
                ViewBag.noteID = testNote.NoteID;
                ViewBag.nid = testNote.SellerID;

                //NoteDetailsViewModel model = new NoteDetailsViewModel
                //{
                //    note = testNote,
                //    country = db.NoteCountries.SingleOrDefault(c => c.ID == testNote.Country),
                //    user = db.Users.SingleOrDefault(c => c.ID == testNote.SellerID),
                //    //noteReview = db.NoteReviews.SingleOrDefault(c => c.NoteID == testNote.NoteID),
                //    //profileUser = db.Profiles.SingleOrDefault(c => c.SellerID == testNote.SellerID)

                //};

               
                List<Note> noteValues = db.Notes.Where(m => m.NoteID == testNote.NoteID).ToList();
                //List<NoteCountry> countryValues = db.NoteCountries.ToList();
                List<User> userValues = db.Users.ToList();
                List<Profile> profileValues = db.Profiles.ToList();
                //List<NoteCategory> categoryValues = db.NoteCategories.ToList();
                List<NoteReview> reviewValues = db.NoteReviews.ToList();

               var model = from n in noteValues
                        join nr in reviewValues on n.NoteID equals nr.NoteID
                        join u in userValues on nr.BuyersID equals u.ID
                        join p in profileValues on u.ID equals p.SellerID
                        select new NoteDetailsViewModel { note = n, user = u, profileUser = p, noteReview = nr};
                        

                return PartialView(model);

            }

            return PartialView();

            
        }

      
        //Dashboard
        public ActionResult Dashboard()
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                //ViewBag.SortOrder = SortOrder;
                List<Note> noteValues = db.Notes.ToList();
                //List<NoteCountry> countryValues = db.NoteCountries.ToList();
                //List<User> userValues = db.Users.ToList();
                //List<DownloadedNote> downloadValues = db.DownloadedNotes.ToList();
                List<NoteCategory> noteCategoryValues = db.NoteCategories.ToList();
                List<ReferenceData> referencesValues = db.ReferenceDatas.ToList();

                var sID = Convert.ToInt32(Session["UserId"]);

                /* var query1 = from n in noteValues
                              join nc in noteCategoryValues on n.Category equals nc.ID
                              join rd in referencesValues on n.Status equals rd.ID
                              where n.SellerID == sID
                              select new NoteDetailsViewModel { note = n, noteCategory = nc, referenceData = rd };



                 return View(query1);*/
                return View();
            }

           
        }


        //Test Dashboard Searching
        
        [ChildActionOnly]
        public ActionResult _InProgressDashboard(string searchingProgress, string SortOrder1, string SortBy1, int PageNumber1 = 1)
        {

            List<Note> noteValues = db.Notes.ToList();
            List<NoteCategory> noteCategoryValues = db.NoteCategories.ToList();
            List<ReferenceData> referencesValues = db.ReferenceDatas.ToList();

            
                ViewData["GetSearchProgess"] = searchingProgress;

                var sID = Convert.ToInt32(Session["UserId"]);

                var query1 = from n in noteValues
                             join nc in noteCategoryValues on n.Category equals nc.ID
                             join rd in referencesValues on n.Status equals rd.ID
                             where n.SellerID == sID
                             select new NoteDetailsViewModel { note = n, noteCategory = nc, referenceData = rd };

                if (!string.IsNullOrEmpty(searchingProgress))
                {
                    query1 = query1.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.NoteTitle.Contains(searchingProgress)
                    || NoteDetailsViewModel.noteCategory.CategoryName.Contains(searchingProgress)
                    || NoteDetailsViewModel.referenceData.Value.Contains(searchingProgress));

                }

                ViewBag.SortOrder = SortOrder1;
                ViewBag.SortBy = SortBy1;
                query1 = ApplySorting(SortOrder1, SortBy1, query1);
                query1 = ApplyPagination(query1, PageNumber1);

                return PartialView(query1);
            

            
        }

        
        [ChildActionOnly]
        //Test Dashboard Searching Published
        public ActionResult _InPublishedDashboard(string searchingPublished, string SortOrder, string SortBy, int PageNumber = 1)
        {
            
                ViewData["GetSearchPublished"] = searchingPublished;

                List<Note> noteValues = db.Notes.ToList();
                List<NoteCategory> noteCategoryValues = db.NoteCategories.ToList();
                List<ReferenceData> referencesValues = db.ReferenceDatas.ToList();

                var sID = Convert.ToInt32(Session["UserId"]);

                var query2 = from n in noteValues
                             join nc in noteCategoryValues on n.Category equals nc.ID
                             join rd in referencesValues on n.Status equals rd.ID
                             where n.SellerID == sID
                             select new NoteDetailsViewModel { note = n, noteCategory = nc, referenceData = rd };

                if (!string.IsNullOrEmpty(searchingPublished))
                {
                    query2 = query2.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.NoteTitle.Contains(searchingPublished)
                    || NoteDetailsViewModel.noteCategory.CategoryName.Contains(searchingPublished)
                    || NoteDetailsViewModel.referenceData.Value.Contains(searchingPublished));
                }

            ViewBag.SortOrder = SortOrder;
            ViewBag.SortBy = SortBy;
            query2 = ApplySorting(SortOrder, SortBy, query2);
            query2 = ApplyPagination(query2, PageNumber);

            return PartialView( query2);
            
        }


        public ActionResult DownloadFlow([Optional]string sEmail, int? bookId)
        {

            ViewBag.v = bookId;

            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("SignUp", "User");
            }
            else
            {
                ViewBag.sellerName = db.Users.FirstOrDefault(m => m.EmailID == sEmail).FirstName;
                var bMail = Session["UserEmailId"].ToString();
                var b1 = db.Users.FirstOrDefault(m => m.EmailID == bMail);
                if (b1 != null)
                {
                    ViewBag.BuyerName = b1.FirstName;
                }

                //BuyerEmail
                var buyerEmail = Session["UserEmailId"].ToString();

                
                
                var fromEmail = new MailAddress("3999vachauhan@gmail.com", "V@chauhan3999");
                var toEmail = new MailAddress(sEmail);
                var fromEmailPassword = "nnumqfegkqbdecgs";

                string subject = db.Users.FirstOrDefault(m => m.EmailID == buyerEmail).FirstName + " wants to purchase your notes";

                string body = "Hello " + db.Users.FirstOrDefault(m => m.EmailID == sEmail).FirstName+ ", <br/> We would like to inform you that "+ db.Users.FirstOrDefault(m => m.EmailID == buyerEmail).FirstName + " wants to purchase your notes. Please see Buyer Request tab and allow download access to Buyer if you received the payment from him. <br/> <br/> Regards,<br/> Notes Marketlace";
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
                };

                using (var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                    smtp.Send(message);

                //DownlodedNote Data Save
                DownloadedNote downlodNote = new DownloadedNote();

                var testNote = db.Notes.FirstOrDefault(m => m.NoteID == bookId);
                if (testNote != null)
                {

                    NoteDetailsViewModel model = new NoteDetailsViewModel
                    {
                        note = testNote,
                        country = db.NoteCountries.SingleOrDefault(c => c.ID == testNote.Country),
                        user = db.Users.SingleOrDefault(c => c.ID == testNote.SellerID),
                        noteCategory = db.NoteCategories.SingleOrDefault(c => c.ID == testNote.Category)
                    };


                    downlodNote.NoteID = model.note.NoteID;
                    downlodNote.SellerID = model.note.SellerID;
                    downlodNote.BuyersID = db.Users.FirstOrDefault(m => m.EmailID == buyerEmail).ID;
                    if (model.note.SellFor == 5)
                    {
                        downlodNote.IsSellerHasAllowedDownload = true;
                        downlodNote.AttachmentPath = model.note.NoteAttachment;
                        downlodNote.IsAttachmentDownloaded = true;
                        downlodNote.IsPaid = true;
                    }
                    else
                    {
                        if (model.note.SellFor == 4)
                        {
                            downlodNote.IsSellerHasAllowedDownload = false;
                            downlodNote.AttachmentPath = null;
                            downlodNote.IsAttachmentDownloaded = false;
                            downlodNote.IsPaid = false;
                        }
                    }
                    // downlodNote.AttachmentDownloadedDate = null;
                    downlodNote.PurchasedPrice = model.note.SellPrice;
                    downlodNote.NoteTitle = model.note.NoteTitle;
                    downlodNote.NoteCategory = model.noteCategory.CategoryName;

                    db.DownloadedNotes.Add(downlodNote);
                    db.SaveChanges();
                    
                }
                var q1 = db.Notes.FirstOrDefault(m => m.NoteID == bookId);
                if (q1 != null)
                {
                    if (q1.SellFor == 5)
                    {
                        //free
                        string downloadFile = q1.NoteAttachment;

                        return File(downloadFile, "application/pdf", downloadFile);
                    }
                    else
                    {
                        if (q1.SellFor == 4)
                        {
                            ViewBag.Error = "Seller did not not allow you to download this note";
                            /*var d1 = db.DownloadedNotes.FirstOrDefault(m => m.NoteID == bookId);
                            if (d1 != null)
                            {
                                if (d1.IsSellerHasAllowedDownload == true)
                                {
                                    string downloadFile = d1.AttachmentPath;

                                    return File(downloadFile, "application/pdf", downloadFile);
                                }
                                else
                                {
                                    ViewBag.Error = "Seller did not not allow you to download this note";
                                }
                            }*/
                        }
                    }
                }


                return RedirectToAction("Dashboard", "User");
            }

            
        }

        

        //Testing Dashboard1
        public ActionResult Dashboard1()
        {
            return View();
        }

        public ActionResult UserProfile()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Index", "User");
            }

            ViewBag.userEmail = Session["UserEmailId"].ToString();
            ViewData["userEmail"] = Session["UserEmailId"];

            var genderList = db.ReferenceDatas.Where(m => m.RefCategory == "Gender").ToList();
            ViewBag.GenderList = new SelectList(genderList, "ID", "Value");

            var countryCodeList = new List<string>() { "+91", "+1", "+44" };
            ViewBag.CountryCodeList = countryCodeList;

            var countryProfileList = new List<string>() { "India", "USA", "UK" };
            ViewBag.CountyProfileList = countryProfileList;

            return View();
        }

        [HttpPost]
        public ActionResult UserProfile(NoteDetailsViewModel n1)
        {
            int userID = Convert.ToInt32(Session["UserId"]);

            var p = db.Profiles.FirstOrDefault(m => m.SellerID == userID);
            if (p == null)
            {
                Profile p1 = new Profile();
                User u1 = new User();

                string emailID = Session["UserEmailId"].ToString();

                var v = db.Users.FirstOrDefault(m => m.ID == userID);
                db.Configuration.ValidateOnSaveEnabled = false;

                if (v != null)
                {
                    v.FirstName = n1.user.FirstName;
                    db.Entry(v).State = System.Data.Entity.EntityState.Modified;

                    v.LastName = n1.user.LastName;
                    db.Entry(v).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
                else
                {
                    ViewBag.err = "No Updates";
                }

                p1.SellerID = Convert.ToInt32(Session["UserId"]);
                p1.DOB = n1.profileUser.DOB;
                p1.Gender = n1.profileUser.Gender;
                p1.PhoneNo = n1.profileUser.PhoneNo;
                p1.CountryCode = n1.profileUser.CountryCode;
                // p1.ProfilePhoto = filename;

                string filename = Path.GetFileNameWithoutExtension(n1.UserImage.FileName);
                //string filename = Path.GetFileNameWithoutExtension(n1.UserImage.FileName);
                string extension = Path.GetExtension(n1.UserImage.FileName);

                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;

                p1.ProfilePhoto = "~/UserPhoto/" + filename;

                filename = Path.Combine(Server.MapPath("~/UserPhoto/"), filename);

                n1.UserImage.SaveAs(filename);

                p1.Address1 = n1.profileUser.Address1;
                p1.Address2 = n1.profileUser.Address2;
                p1.City = n1.profileUser.City;
                p1.State = n1.profileUser.State;
                p1.ZipCode = n1.profileUser.ZipCode;
                p1.Country = n1.profileUser.Country;
                p1.University = n1.profileUser.University;
                p1.College = n1.profileUser.College;
                p1.IsActive = true;

                db.Profiles.Add(p1);
                db.SaveChanges();
            }
            else
            {
                string emailID = Session["UserEmailId"].ToString();

                var v = db.Users.FirstOrDefault(m => m.ID == userID);
                db.Configuration.ValidateOnSaveEnabled = false;

                if (v != null)
                {
                    v.FirstName = n1.user.FirstName;
                    db.Entry(v).State = System.Data.Entity.EntityState.Modified;

                    v.LastName = n1.user.LastName;
                    db.Entry(v).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
                else
                {
                    ViewBag.err = "No Updates";
                }

                p.SellerID = Convert.ToInt32(Session["UserId"]);
                p.DOB = n1.profileUser.DOB;
                p.Gender = n1.profileUser.Gender;
                p.PhoneNo = n1.profileUser.PhoneNo;
                p.CountryCode = n1.profileUser.CountryCode;
                // p1.ProfilePhoto = filename;

                string filename = Path.GetFileNameWithoutExtension(n1.UserImage.FileName);
                //string filename = Path.GetFileNameWithoutExtension(n1.UserImage.FileName);
                string extension = Path.GetExtension(n1.UserImage.FileName);

                filename = filename + DateTime.Now.ToString("yymmssfff") + extension;

                p.ProfilePhoto = "~/UserPhoto/" + filename;

                filename = Path.Combine(Server.MapPath("~/UserPhoto/"), filename);

                n1.UserImage.SaveAs(filename);

                p.Address1 = n1.profileUser.Address1;
                p.Address2 = n1.profileUser.Address2;
                p.City = n1.profileUser.City;
                p.State = n1.profileUser.State;
                p.ZipCode = n1.profileUser.ZipCode;
                p.Country = n1.profileUser.Country;
                p.University = n1.profileUser.University;
                p.College = n1.profileUser.College;
                p.IsActive = true;

                //db.Profiles.Add(p1);
                db.SaveChanges();
            }

            
           

            var genderList = db.ReferenceDatas.Where(m => m.RefCategory == "Gender").ToList();
            ViewBag.GenderList = new SelectList(genderList, "ID", "Value");

            var countryCodeList = new List<string>() { "+91", "+1", "+44" };
            ViewBag.CountryCodeList = countryCodeList;

            var countryProfileList = new List<string>() { "India", "USA", "UK" };
            ViewBag.CountyProfileList = countryProfileList;

           

           

           



            return RedirectToAction("ContactUs");

        }

        


        public ActionResult MyDownload(string SortOrder, string SortBy, int PageNumber = 1)
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                List<Note> noteValues = db.Notes.ToList();
                //List<NoteCountry> countryValues = db.NoteCountries.ToList();
                List<User> userValues = db.Users.ToList();
                var downloadValues = db.DownloadedNotes.Where(m => m.IsSellerHasAllowedDownload == true).ToList();
                List<ReferenceData> referenceDataValues = db.ReferenceDatas.ToList();
                List<NoteCategory> categoryValues = db.NoteCategories.ToList();

                var sID = Convert.ToInt32(Session["UserId"]);

                var query1 = from u in userValues
                             join d in downloadValues on u.ID equals d.BuyersID
                             join n in noteValues on d.NoteID equals n.NoteID
                             join nc in categoryValues on n.Category equals nc.ID
                             join rd in referenceDataValues on n.SellFor equals rd.ID
                             where d.BuyersID == sID
                             select new NoteDetailsViewModel { note = n, user = u, download = d, noteCategory = nc, referenceData = rd };

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;
                query1 = ApplySorting(SortOrder, SortBy, query1);
                query1 = ApplyPagination(query1, PageNumber);


                return View(query1);
            }

        }

        //Download book from MyDownloads
        public ActionResult DownLoadNote(int dID)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            var v = db.DownloadedNotes.FirstOrDefault(m => m.ID == dID);
            if (v != null)
            {
                //update
                v.IsAttachmentDownloaded = true;
                db.Entry(v).State = System.Data.Entity.EntityState.Modified;

                v.AttachmentDownloadedDate = DateTime.Today;
                db.Entry(v).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();

                string downloadFile = v.AttachmentPath;

                return File(downloadFile, "application/pdf", downloadFile);
            }

            return View();
        }

        [HttpPost]
        public ActionResult MyDownload(string searching, string SortOrder, string SortBy, int PageNumber = 1)
        {
            ViewData["GrtDownloadDetails"] = searching;


            List<Note> noteValues = db.Notes.ToList();
            //List<NoteCountry> countryValues = db.NoteCountries.ToList();
            List<User> userValues = db.Users.ToList();
            var downloadValues = db.DownloadedNotes.ToList();
            List<ReferenceData> referenceDataValues = db.ReferenceDatas.ToList();
            List<NoteCategory> categoryValues = db.NoteCategories.ToList();

            var sID = Convert.ToInt32(Session["UserId"]);

            var query1 = from u in userValues
                         join d in downloadValues on u.ID equals d.BuyersID
                         join n in noteValues on d.NoteID equals n.NoteID
                         join nc in categoryValues on n.Category equals nc.ID
                         join rd in referenceDataValues on n.SellFor equals rd.ID
                         where d.BuyersID == sID
                         select new NoteDetailsViewModel { note = n, user = u, download = d, noteCategory = nc, referenceData = rd };


            if (!string.IsNullOrEmpty(searching))
            {
                query1 = query1.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.NoteTitle.Contains(searching)
                || NoteDetailsViewModel.note.CourseName.Contains(searching));
                query1 = ApplySorting(SortOrder, SortBy, query1);
                query1 = ApplyPagination(query1, PageNumber);
            }

            return View("MyDownload", query1);
        }

        //[HttpPost]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult NoteReview(int? dID,string ratingComment, string rate, string SortOrder, string SortBy, int PageNumber = 1)
        {
            NoteReview reviewOfNote = new NoteReview();

            ViewBag.downloadID = dID;
            ViewBag.star = rate;
            ViewBag.value = ratingComment;

            var v = db.DownloadedNotes.FirstOrDefault(m => m.ID == dID);

            db.Configuration.ValidateOnSaveEnabled = false;

            var p = db.NoteReviews.FirstOrDefault(m => m.AgainstDownloadsID == dID);
            if (p == null)
            {
                if (v != null)
                {
                    reviewOfNote.NoteID = v.NoteID;
                    reviewOfNote.Ratings = Convert.ToDecimal(rate);
                    reviewOfNote.Feedback = ratingComment;
                    reviewOfNote.AgainstDownloadsID = v.ID;
                    reviewOfNote.Inappropriate = false;
                    reviewOfNote.BuyersID = Convert.ToInt32(Session["UserId"]);
                    reviewOfNote.CreatedDate = DateTime.Now;
                    reviewOfNote.IsActive = true;

                    db.NoteReviews.Add(reviewOfNote);
                    db.SaveChanges();
                }

            }
            else
            {
                p.Ratings = Convert.ToDecimal(rate);
                p.Feedback = ratingComment;
                p.ModifiedDate = DateTime.Now;

                db.SaveChanges();
            }

            List<Note> noteValues = db.Notes.ToList();
            //List<NoteCountry> countryValues = db.NoteCountries.ToList();
            List<User> userValues = db.Users.ToList();
            List<DownloadedNote> downloadValues = db.DownloadedNotes.Where(m => m.IsSellerHasAllowedDownload == true).ToList();
            List<ReferenceData> referenceDataValues = db.ReferenceDatas.ToList();
            List<NoteCategory> categoryValues = db.NoteCategories.ToList();

            var sID = Convert.ToInt32(Session["UserId"]);

            var query1 = from u in userValues
                         join d in downloadValues on u.ID equals d.BuyersID
                         join n in noteValues on d.NoteID equals n.NoteID
                         join nc in categoryValues on n.Category equals nc.ID
                         join rd in referenceDataValues on n.SellFor equals rd.ID
                         where d.BuyersID == sID
                         select new NoteDetailsViewModel { note = n, user = u, download = d, noteCategory = nc, referenceData = rd };

            ViewBag.SortOrder = SortOrder;
            ViewBag.SortBy = SortBy;
            query1 = ApplySorting(SortOrder, SortBy, query1);
            query1 = ApplyPagination(query1, PageNumber);


            return View("MyDownload", query1);

        }

        public ActionResult MarkInapropriate(int? dID, string SortOrder, string SortBy, int PageNumber = 1)
        {


            var v = db.NoteReviews.FirstOrDefault(m => m.AgainstDownloadsID == dID);
            if (v != null)
            {
                v.Inappropriate = true;
                v.ModifiedDate = DateTime.Now;
                db.SaveChanges();
            }

            List<Note> noteValues = db.Notes.ToList();
            //List<NoteCountry> countryValues = db.NoteCountries.ToList();
            List<User> userValues = db.Users.ToList();
            List<DownloadedNote> downloadValues = db.DownloadedNotes.Where(m => m.IsSellerHasAllowedDownload == true).ToList();
            List<ReferenceData> referenceDataValues = db.ReferenceDatas.ToList();
            List<NoteCategory> categoryValues = db.NoteCategories.ToList();

            var sID = Convert.ToInt32(Session["UserId"]);

            var query1 = from u in userValues
                         join d in downloadValues on u.ID equals d.BuyersID
                         join n in noteValues on d.NoteID equals n.NoteID
                         join nc in categoryValues on n.Category equals nc.ID
                         join rd in referenceDataValues on n.SellFor equals rd.ID
                         where d.BuyersID == sID
                         select new NoteDetailsViewModel { note = n, user = u, download = d, noteCategory = nc, referenceData = rd };

            ViewBag.SortOrder = SortOrder;
            ViewBag.SortBy = SortBy;
            query1 = ApplySorting(SortOrder, SortBy, query1);
            query1 = ApplyPagination(query1, PageNumber);

            var uid = Convert.ToInt32(Session["UserId"]);

            var vi = db.DownloadedNotes.FirstOrDefault(m => m.ID == dID);

            var fromEmail = new MailAddress("3999vachauhan@gmail.com", "V@chauhan3999");
            var toEmail = new MailAddress("pateldhara1019@gmail.com", "Dhara1112");
            var fromEmailPassword = "nnumqfegkqbdecgs";

            string subject = db.Users.FirstOrDefault(m => m.ID == uid).FirstName + " Reported an issue for " + db.Notes.FirstOrDefault(m => m.NoteID == vi.NoteID).NoteTitle;

            string body = "Hello Admin, <br/> <br/>We want to inform you that, "+ db.Users.FirstOrDefault(m => m.ID == uid).FirstName + " Reported an issue for " +  db.Users.FirstOrDefault(m => m.ID == vi.SellerID).FirstName +"'s Note with title "+ db.Notes.FirstOrDefault(m => m.NoteID == vi.NoteID).NoteTitle + ". Please look at the notes and takerequired action.<br/><br/> Regards," + "<br/> Notes Marketplace";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
            
            return View("MyDownload", query1);

        }

        //Sold Note
        public ActionResult MySoldNote(string SortOrder, string SortBy, int PageNumber = 1)
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                List<Note> noteValues = db.Notes.ToList();
                //List<NoteCountry> countryValues = db.NoteCountries.ToList();
                List<User> userValues = db.Users.ToList();
                List<DownloadedNote> downloadValues = db.DownloadedNotes.Where(m => m.IsSellerHasAllowedDownload == true).ToList();
                List<ReferenceData> referenceDataValues = db.ReferenceDatas.ToList();
                List<NoteCategory> categoryValues = db.NoteCategories.ToList();

                var sID = Convert.ToInt32(Session["UserId"]);

                var query1 = from u in userValues
                             join d in downloadValues on u.ID equals d.BuyersID
                             join n in noteValues on d.NoteID equals n.NoteID
                             join nc in categoryValues on n.Category equals nc.ID
                             join rd in referenceDataValues on n.SellFor equals rd.ID
                             where n.SellerID == sID
                             select new NoteDetailsViewModel { note = n, user = u, download = d, noteCategory = nc, referenceData = rd };

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;
                query1 = ApplySorting(SortOrder, SortBy, query1);
                query1 = ApplyPagination(query1, PageNumber);


                return View(query1);
            }

        }

        [HttpPost]
        public ActionResult MySoldNote(string searching, string SortOrder, string SortBy, int PageNumber = 1)
        {
            ViewData["GetSoldNoteDetails"] = searching;


            List<Note> noteValues = db.Notes.ToList();
            //List<NoteCountry> countryValues = db.NoteCountries.ToList();
            List<User> userValues = db.Users.ToList();
            var downloadValues = db.DownloadedNotes.ToList();
            List<ReferenceData> referenceDataValues = db.ReferenceDatas.ToList();
            List<NoteCategory> categoryValues = db.NoteCategories.ToList();

            var sID = Convert.ToInt32(Session["UserId"]);

            var query1 = from u in userValues
                         join d in downloadValues on u.ID equals d.BuyersID
                         join n in noteValues on d.NoteID equals n.NoteID
                         join nc in categoryValues on n.Category equals nc.ID
                         join rd in referenceDataValues on n.SellFor equals rd.ID
                         where n.SellerID == sID
                         select new NoteDetailsViewModel { note = n, user = u, download = d, noteCategory = nc, referenceData = rd };


            if (!string.IsNullOrEmpty(searching))
            {
                query1 = query1.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.NoteTitle.Contains(searching)
                || NoteDetailsViewModel.note.CourseName.Contains(searching));
                query1 = ApplySorting(SortOrder, SortBy, query1);
                query1 = ApplyPagination(query1, PageNumber);
            }

            return View("MySoldNote", query1);
        }


        public ActionResult ChangePassword()
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {

                return View();
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string conformPassword)
        {
            
                var uID = Convert.ToInt32(Session["UserId"]);

                var q1 = db.Users.FirstOrDefault(m => m.ID == uID);
                if(q1 != null)
                {
                    if(q1.Password == oldPassword)
                    {
                        if(newPassword == conformPassword)
                        {
                            db.Configuration.ValidateOnSaveEnabled = false;
                            q1.Password = newPassword;

                            db.SaveChanges();

                            return RedirectToAction("Index", "User");
                        }
                        else
                        {
                            ViewBag.matchPassword = "Password Not Match";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.matchPassword1 = "Old Password Not Match";
                        return View();
                    }
                }
            

            return View();
        }

        
        public ActionResult MyRejectedNote(string SortOrder, string SortBy, int PageNumber = 1)
        {
            if (Session["UserEmailId"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                List<Note> noteValues = db.Notes.Where(m => m.Status == 10).ToList();
                //List<NoteCountry> countryValues = db.NoteCountries.ToList();
                List<User> userValues = db.Users.ToList();
                List<DownloadedNote> downloadValues = db.DownloadedNotes.Where(m => m.IsSellerHasAllowedDownload == true).ToList();
                List<ReferenceData> referenceDataValues = db.ReferenceDatas.ToList();
                List<NoteCategory> categoryValues = db.NoteCategories.ToList();

                var sID = Convert.ToInt32(Session["UserId"]);

                var query1 = from u in userValues
                             join d in downloadValues on u.ID equals d.BuyersID
                             join n in noteValues on d.NoteID equals n.NoteID
                             join nc in categoryValues on n.Category equals nc.ID
                             join rd in referenceDataValues on n.SellFor equals rd.ID
                             where n.SellerID == sID
                             select new NoteDetailsViewModel { note = n, user = u, download = d, noteCategory = nc, referenceData = rd };

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;
                query1 = ApplySorting(SortOrder, SortBy, query1);
                query1 = ApplyPagination(query1, PageNumber);


                return View(query1);
            }
        }


        [HttpPost]
        public ActionResult MyRejectedNote(string searching, string SortOrder, string SortBy, int PageNumber = 1)
        {
            ViewData["GetRejectedNoteDetails"] = searching;


            List<Note> noteValues = db.Notes.Where(m => m.Status == 10).ToList();
            //List<NoteCountry> countryValues = db.NoteCountries.ToList();
            List<User> userValues = db.Users.ToList();
            var downloadValues = db.DownloadedNotes.ToList();
            List<ReferenceData> referenceDataValues = db.ReferenceDatas.ToList();
            List<NoteCategory> categoryValues = db.NoteCategories.ToList();

            var sID = Convert.ToInt32(Session["UserId"]);

            var query1 = from u in userValues
                         join d in downloadValues on u.ID equals d.BuyersID
                         join n in noteValues on d.NoteID equals n.NoteID
                         join nc in categoryValues on n.Category equals nc.ID
                         join rd in referenceDataValues on n.SellFor equals rd.ID
                         where n.SellerID == sID
                         select new NoteDetailsViewModel { note = n, user = u, download = d, noteCategory = nc, referenceData = rd };


            if (!string.IsNullOrEmpty(searching))
            {
                query1 = query1.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.NoteTitle.Contains(searching)
                || NoteDetailsViewModel.note.CourseName.Contains(searching));
                query1 = ApplySorting(SortOrder, SortBy, query1);
                query1 = ApplyPagination(query1, PageNumber);
            }

            return View("MySoldNote", query1);
        }

        //Delete file 
        public ActionResult DeleteConform(int deleteId)
        {
            var del = db.Notes.FirstOrDefault(m => m.NoteID == deleteId);
            if(del != null)
            {
                db.Notes.Remove(del);
                db.SaveChanges();
            }

            return View("Dashboard");
        }

        // Non Action methid for forgetpassword
        [NonAction]
        public void sendEmail(string emailID, string pass, string emailfor = "VerifyAccount")
        {
            var fromEmail = new MailAddress("3999vachauhan@gmail.com", "V@chauhan3999");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "nnumqfegkqbdecgs";

            string numbers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz!@#$%&*";


            Random objrandom = new Random();

            string passwordStrig = "";
            string strrandom = string.Empty;
            for(int i=0; i < 8; i++)
            {
                int temp = objrandom.Next(0, numbers.Length);
                passwordStrig = numbers.ToCharArray()[temp].ToString();
                strrandom += passwordStrig;
            }

            pass = strrandom;


            var data = db.Users.FirstOrDefault(m => m.EmailID == emailID);
                //Where(m => m.EmailID == emailID).FirstOrDefault();
            if (data != null)
            {
                
                data.Password = strrandom;
                db.SaveChanges();
            }
           
            string subject = "New Tempoary Password has been created for you";

            string body = "Hello,<br/>We have generated a new password for you<br/>Password:" +  pass +"<br/>Regards,<br/>Note Marketplace";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);

        }


        // Non Action methid for Is email exist or not
        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            var v = db.Users.Where(a => a.EmailID == emailID).FirstOrDefault();
            return v != null;
        }


        // Non Action methid for Email verification
        [NonAction]
        public void sendVerificationLinkEmail(string emailID, string Fname)
        {
            //hoe action
            var verifyurl = "/User/EmailVerification/?emailID="+ emailID;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyurl);

            var fromEmail = new MailAddress("3999vachauhan@gmail.com", "V@chauhan3999");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "nnumqfegkqbdecgs";

            string subject = "NoteMarketplace - Email Verification";

            string body = "Hello "+Fname +",<br/>"+"Thank you for signing up with us. Please click on below link to verify your email address and to do login.<br/>"+"<a href='"+link+"'>"+link+"</a>"+"<br/>Regards,<br/>Notes Marketplace";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);

        }

        public IEnumerable<NoteDetailsViewModel> ApplySorting(string SortOrder,string  SortBy, IEnumerable<NoteDetailsViewModel> query1)
        {
            switch (SortBy)
            {
                case "Note Title":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.note.NoteTitle).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.note.NoteTitle).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.note.NoteTitle).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Title":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.note.NoteTitle).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.note.NoteTitle).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.note.NoteTitle).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Category":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.noteCategory.CategoryName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.noteCategory.CategoryName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.noteCategory.CategoryName).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Buyer":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.EmailID).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.EmailID).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.EmailID).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Sell Type":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.referenceData.Value).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.referenceData.Value).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.referenceData.Value).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Price":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.note.SellPrice).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.note.SellPrice).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.note.SellPrice).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Status":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.referenceData.Value).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.referenceData.Value).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.referenceData.Value).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        query1 = query1.OrderBy(x => x.note.NoteTitle).ToList();
                        break;
                    }
            }
            return query1;
        }

        public IEnumerable<NoteDetailsViewModel> ApplyPagination(IEnumerable<NoteDetailsViewModel> query1, int PageNumber = 1)
        {
            ViewBag.PageTotal = Math.Ceiling(query1.Count() / 1.0);

            ViewBag.TotalRecord = query1.Count();
            ViewBag.PageNumber = PageNumber;

            query1 = query1.Skip((PageNumber - 1) * 1).Take(1).ToList();

            return query1;
        }

    }
}


/* public ActionResult NoteDetails(string noteName)
         {
             //ViewBag.name = noteName;
              

             var testNote = db.Notes.FirstOrDefault(m => m.NoteTitle == noteName);
             if (testNote != null)
             {
                ViewBag.na = noteName;
                 ViewBag.noteID = testNote.NoteID;
                Session["nID"] = testNote.NoteID;
                 List <Note> noteValues = db.Notes.ToList();
                List<NoteCountry> countryValues = db.NoteCountries.ToList();

               //var NoteInformation = from n in noteValues
                 //                     where n.NoteTitle == "Computer Science"
                  //                     select new Note { note1 = n};

                var NoteInformation = from n in noteValues
                                         join cv in countryValues on n.Country equals cv.ID
                                       // where n.NoteID == 4
                                       select new NoteCountryContex { countryDetails = cv, noteDetails = n };

                return View(NoteInformation);
            }

            else
             {
                ViewBag.na = "No";
                 return View();
             }
         }*/

/* [Route("User/DownloadNotes/{id}")]
      public ActionResult DownloadNotes(int? id)
       {
           var q2 = db.Users.FirstOrDefault(m => m.ID == id);
           ViewBag.v = "Yes";
           if (q2 != null)
           {
               ViewBag.v = q2.EmailID;
           }
           else 
           {
               ViewBag.v = "No";
           }

            return RedirectToAction("Dashboard", "User");



       }*/

/*[HttpPost]
[Route("User/NoteDetails/{id}")]
public ActionResult NoteDetails(int? id)
{
    if (Session["UserEmailId"] != null)
    {
        return RedirectToAction("SignUp", "User");
    }

    else
    {
        ViewBag.sId = id;

        var Q1 = db.Users.FirstOrDefault(m => m.ID == id);

        if (Q1 != null)
        {
            ViewBag.emailOfSeller = Q1.EmailID;
        }

var fromEmail = new MailAddress("3999vachauhan@gmail.com", "V@chauhan3999");
        var toEmail = new MailAddress("pateldhara1019@gmail.com", "Dhara1112");
        var fromEmailPassword = "nnumqfegkqbdecgs";

        string subject =  "wants to purchase your notes";

        string body = "Hello, <br/> We would like to inform you that <buyer name> wants to purchase your notes. Please see Buyer Request tab and allow download access to Buyer if you received the payment from him. <br/> <br/> Regards,<br/> Notes Marketlace" ;

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
        };

        using (var message = new MailMessage(fromEmail, toEmail)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        })
            smtp.Send(message);

    }
    return View();

}*/

//[HttpPost]
//public ActionResult UserProfile(NoteDetailsViewModel p1)
//{
//    var genderList = db.ReferenceDatas.Where(m => m.RefCategory == "Gender").ToList();
//    ViewBag.GenderList = new SelectList(genderList, "ID", "Value");

//    var countryCodeList = new List<string>() { "+91", "+1", "+44" };
//    ViewBag.CountryCodeList = countryCodeList;

//    var countryProfileList = new List<string>() { "India", "USA", "UK" };
//    ViewBag.CountyProfileList = countryProfileList;

//    if (ModelState.IsValid)
//    {

//        if (p1.profileUser.ProfilePhoto == null)
//        {
//            p1.profileUser.ProfilePhoto = "";
//        }
//        else
//        {
//            //Image upload for book image
//            string filename = Path.GetFileNameWithoutExtension(p1.UserImage.FileName);
//            string extension = Path.GetExtension(p1.UserImage.FileName);

//            filename = filename + DateTime.Now.ToString("yymmssfff") + extension;

//            p1.profileUser.ProfilePhoto = "~/UserPhoto/" + filename;

//            filename = Path.Combine(Server.MapPath("~/UserPhoto/"), filename);

//            p1.UserImage.SaveAs(filename);
//        }

//        Profile profile = new Profile();
//        User u1 = new User();


//        u1.FirstName = p1.user.FirstName;
//        u1.LastName = p1.user.LastName;

//        profile.SellerID = Convert.ToInt32(Session["UserId"]);
//        profile.DOB = p1.profileUser.DOB;

//        profile.Gender = p1.profileUser.Gender;

//        profile.PhoneNo = p1.profileUser.PhoneNo;
//        profile.CountryCode = p1.profileUser.CountryCode;
//        profile.Address1 = p1.profileUser.Address1;
//        profile.Address2 = p1.profileUser.Address2;
//        profile.City = p1.profileUser.City;
//        profile.State = p1.profileUser.State;
//        profile.ZipCode = p1.profileUser.ZipCode;
//        profile.Country = p1.profileUser.Country;
//        profile.University = p1.profileUser.University;
//        profile.College = p1.profileUser.College;
//        profile.IsActive = true;

//        db.Profiles.Add(profile);

//        db.SaveChanges();
//    }
//    return RedirectToAction("ContactUs");
//}