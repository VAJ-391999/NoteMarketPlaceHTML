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
    public class AdminController : Controller
    {
        NoteMarketPlaceDatabaseEntities db = new NoteMarketPlaceDatabaseEntities();

        // GET: User Logout Page 
        public ActionResult logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Admin");
        }

        //Admin Dashboard
        public ActionResult AdminDashboard( int? noteId, string selectMonth, string SortOrder, string SortBy, int PageNumber = 1)
        {
            if(Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                //ViewBag.month = DateTime.Now.ToString("MM/dd/yyyy HH':'mm");

                var monthList = new List<string>() { "January", "Feb", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                ViewBag.MonthList = monthList;

                List<Note> noteValues = db.Notes.Where(m => m.Status == 9).ToList();
                List<ReferenceData> referenceValues = db.ReferenceDatas.ToList();
                List<User> userValues = db.Users.ToList();
                List<DownloadedNote> downloadValues = db.DownloadedNotes.Distinct().ToList();
                List<NoteCategory> categoryValues = db.NoteCategories.ToList();
                

                var adminQuery = from u in userValues
                                 join n in noteValues on u.ID equals n.SellerID
                                 join cv in categoryValues on n.Category equals cv.ID
                                 join rd in referenceValues on n.SellFor equals rd.ID
                                 //join dn in downloadValues on n.NoteID equals dn.NoteID 
                                 select new NoteDetailsViewModel { note = n, user = u, referenceData = rd, noteCategory = cv/*, download = dn count = p.Select(m => m.NoteID)*/ };

                
                if (!string.IsNullOrEmpty(selectMonth))
                {
                    adminQuery = from u in userValues
                                 join n in noteValues on u.ID equals n.SellerID
                                 join cv in categoryValues on n.Category equals cv.ID
                                 join rd in referenceValues on n.SellFor equals rd.ID
                                 where DateTime.ParseExact(n.PublishedDate.ToString(),"dd-MM-yyyy h'.'mm'.'ss tt", null).ToString("MMMM") == selectMonth 
                                 select new NoteDetailsViewModel { note = n, user = u, referenceData = rd, noteCategory = cv/*, download = dn count = p.Select(m => m.NoteID)*/ };
                }

                if (noteId != null)
                {
                    var dnote = db.Notes.FirstOrDefault(m => m.NoteID == noteId);
                    if (dnote != null)
                    {
                        string downloadFile = dnote.NoteAttachment;

                        return File(downloadFile, "application/pdf", downloadFile);
                    }
                }

                

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;

                adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);

                //return RedirectToAction("AdminDashboard", adminQuery);

                return View(adminQuery);
            }

            
        }

        [HttpPost]
        public async Task<ActionResult> AdminDashboard(int? unpublishNote , string removedRemarks, string searching,string SortOrder, string SortBy, int PageNumber = 1)
        {

            if (unpublishNote != null)
            {
                var temp = db.Notes.FirstOrDefault(m => m.NoteID == unpublishNote);
                var tempuser = db.Users.FirstOrDefault(m => m.ID == temp.SellerID);

                var sID = Convert.ToInt32(Session["UserId"]);

                if (temp != null)
                {
                    temp.ActionedBy = sID;
                    temp.Status = 11;
                    temp.AdminRemarks = removedRemarks;

                    db.Entry(temp).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                }
                EmailForUnpublishNote(unpublishNote, tempuser.EmailID);
            }

            ViewData["GetpublishedNotes"] = searching;

            var monthList = new List<string>() { "January", "Feb", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            ViewBag.MonthList = monthList;

            List<Note> noteValues = db.Notes.Where(m => m.Status == 9).ToList();
            List<ReferenceData> referenceValues = db.ReferenceDatas.ToList();
            List<User> userValues = db.Users.ToList();
            List<DownloadedNote> downloadValues = db.DownloadedNotes.ToList();
            List<NoteCategory> categoryValues = db.NoteCategories.ToList();


            var adminQuery = from u in userValues
                             join n in noteValues on u.ID equals n.SellerID
                             join cv in categoryValues on n.Category equals cv.ID
                             join rd in referenceValues on n.SellFor equals rd.ID
                             //join dn in downloadValues on n.NoteID equals dn.NoteID
                             select new NoteDetailsViewModel { note = n, user = u, referenceData = rd, noteCategory = cv/*, download = dn count = p.Select(m => m.NoteID)*/ };


            

            if (!string.IsNullOrEmpty(searching))
            {
                adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.NoteTitle.ToLower().StartsWith(searching.ToLower())
                || NoteDetailsViewModel.noteCategory.CategoryName.ToLower().StartsWith(searching.ToLower())
                //|| NoteDetailsViewModel.note.SellPrice.Equals(Convert.ToInt32(searching))
                || NoteDetailsViewModel.referenceData.Value.ToLower().StartsWith(searching.ToLower())
                || NoteDetailsViewModel.user.FirstName.ToLower().StartsWith(searching.ToLower())
                );

                adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);
            }

            adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
            adminQuery = ApplyPagination(adminQuery, PageNumber);

            return View("AdminDashboard", adminQuery);
            
        }



        //Admin Notes Under Reviews
        public ActionResult NotesUnderReview(int? nID,int? noteId, int? userKey, int? sellerValue, string SortOrder, string SortBy, int PageNumber = 1)
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                ViewBag.sID = sellerValue;

                var sellerList = db.Users.Where(m => m.RoleID == 1).ToList();
                ViewBag.SellerList = new SelectList(sellerList, "ID", "FirstName");

                List<Note> noteValues = db.Notes.Where(m => m.Status == 7 || m.Status == 8).ToList();
                List<ReferenceData> referenceValues = db.ReferenceDatas.ToList();
                List<User> userValues = db.Users.ToList();
                List<DownloadedNote> downloadValues = db.DownloadedNotes.Distinct().ToList();
                List<NoteCategory> categoryValues = db.NoteCategories.ToList();


                var adminQuery = from u in userValues
                                 join n in noteValues on u.ID equals n.SellerID
                                 join cv in categoryValues on n.Category equals cv.ID
                                 join rd in referenceValues on n.Status equals rd.ID
                                 //join dn in downloadValues on n.NoteID equals dn.NoteID 
                                 select new NoteDetailsViewModel { note = n, user = u, referenceData = rd, noteCategory = cv/*, download = dn count = p.Select(m => m.NoteID)*/ };

                var sID = Convert.ToInt32(Session["UserId"]);

                if (sellerValue != null)
                {
                    adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.user.ID == sellerValue);

                    adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                    adminQuery = ApplyPagination(adminQuery, PageNumber);
                }

                if(userKey != null)
                {
                    adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.user.ID == userKey);

                    //adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                    //adminQuery = ApplyPagination(adminQuery, PageNumber);
                }

                if (noteId != null)
                {
                    var dnote = db.Notes.FirstOrDefault(m => m.NoteID == noteId);
                    if (dnote != null)
                    {
                        string downloadFile = dnote.NoteAttachment;

                        return File(downloadFile, "application/pdf", downloadFile);
                    }
                }

                if(nID != null)
                {
                    ApprovedByAdmin(nID);
                }

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;

                adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);


                return View(adminQuery);
            }
        }

        [HttpPost]
        public ActionResult NotesUnderReview(string searching, string SortOrder, string SortBy, int PageNumber = 1)
        {
            var sellerList = db.Users.Where(m => m.RoleID == 1).ToList();
            ViewBag.SellerList = new SelectList(sellerList, "ID", "FirstName");


            ViewData["GetReviewNotes"] = searching;

            List<Note> noteValues = db.Notes.Where(m => m.Status == 7 || m.Status == 8).ToList();
            List<ReferenceData> referenceValues = db.ReferenceDatas.ToList();
            List<User> userValues = db.Users.ToList();
            List<DownloadedNote> downloadValues = db.DownloadedNotes.ToList();
            List<NoteCategory> categoryValues = db.NoteCategories.ToList();


            var adminQuery = from u in userValues
                             join n in noteValues on u.ID equals n.SellerID
                             join cv in categoryValues on n.Category equals cv.ID
                             join rd in referenceValues on n.Status equals rd.ID
                             //join dn in downloadValues on n.NoteID equals dn.NoteID
                             select new NoteDetailsViewModel { note = n, user = u, referenceData = rd, noteCategory = cv/*, download = dn count = p.Select(m => m.NoteID)*/ };

            
            if (!string.IsNullOrEmpty(searching))
            {
                adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.NoteTitle.Contains(searching)
                || NoteDetailsViewModel.noteCategory.CategoryName.Contains(searching)

                );

                adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);
            }

            return View("NotesUnderReview", adminQuery);

            
        }



        //published note
        public ActionResult publishedNotes(int? userKey,int? noteId, int? sellerValue, string SortOrder, string SortBy, int PageNumber = 1)
        {

            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                ViewBag.sID = sellerValue;

                var sellerList = db.Users.Where(m => m.RoleID == 1).ToList();
                ViewBag.SellerList = new SelectList(sellerList, "ID", "FirstName");

                List<Note> noteValues = db.Notes.Where(m => m.Status == 9).ToList();
                List<ReferenceData> referenceValues = db.ReferenceDatas.ToList();
                List<User> userValues = db.Users.ToList();
                List<DownloadedNote> downloadValues = db.DownloadedNotes.Distinct().ToList();
                List<NoteCategory> categoryValues = db.NoteCategories.ToList();


                var adminQuery = from u in userValues
                                 join n in noteValues on u.ID equals n.SellerID
                                 join cv in categoryValues on n.Category equals cv.ID
                                 join rd in referenceValues on n.SellFor equals rd.ID
                                 //join dn in downloadValues on u.ID equals dn.BuyersID 
                                 select new NoteDetailsViewModel { note = n, user = u, referenceData = rd, noteCategory = cv/*, download = dn count = p.Select(m => m.NoteID)*/ };

                var sID = Convert.ToInt32(Session["UserId"]);

                if (sellerValue != null)
                {
                    adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.user.ID == sellerValue);

                    adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                    adminQuery = ApplyPagination(adminQuery, PageNumber);
                }

                if (noteId != null)
                {
                    var dnote = db.Notes.FirstOrDefault(m => m.NoteID == noteId);
                    if (dnote != null)
                    {
                        string downloadFile = dnote.NoteAttachment;

                        return File(downloadFile, "application/pdf", downloadFile);
                    }
                }

                if (userKey != null)
                {
                    adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.SellerID == userKey);

                }

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;

                adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);


                return View(adminQuery);
            }
        }

        [HttpPost]
        public ActionResult publishedNotes(int? unpublishNote, string removedRemarks, string searching, string SortOrder, string SortBy, int PageNumber = 1)
        {
            if (unpublishNote != null)
            {
                var temp = db.Notes.FirstOrDefault(m => m.NoteID == unpublishNote);
                var tempuser = db.Users.FirstOrDefault(m => m.ID == temp.SellerID);

                var sID = Convert.ToInt32(Session["UserId"]);

                if (temp != null)
                {
                    temp.ActionedBy = sID;
                    temp.Status = 11;
                    temp.AdminRemarks = removedRemarks;

                    db.Entry(temp).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                }
                EmailForUnpublishNote(unpublishNote, tempuser.EmailID);
            }

            var sellerList = db.Users.Where(m => m.RoleID == 1).ToList();
            ViewBag.SellerList = new SelectList(sellerList, "ID", "FirstName");


            ViewData["GetReviewNotes"] = searching;

            List<Note> noteValues = db.Notes.Where(m => m.Status == 9).ToList();
            List<ReferenceData> referenceValues = db.ReferenceDatas.ToList();
            List<User> userValues = db.Users.ToList();
            List<DownloadedNote> downloadValues = db.DownloadedNotes.ToList();
            List<NoteCategory> categoryValues = db.NoteCategories.ToList();


            var adminQuery = from u in userValues
                             join n in noteValues on u.ID equals n.SellerID
                             join cv in categoryValues on n.Category equals cv.ID
                             join rd in referenceValues on n.SellFor equals rd.ID
                             //join dn in downloadValues on u.ID equals dn.BuyersID
                             select new NoteDetailsViewModel { note = n, user = u, referenceData = rd, noteCategory = cv/*, download = dn count = p.Select(m => m.NoteID)*/ };


            if (!string.IsNullOrEmpty(searching))
            {
                adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.NoteTitle.Contains(searching)
                || NoteDetailsViewModel.noteCategory.CategoryName.Contains(searching)
                //|| NoteDetailsViewModel.note.SellPrice.Equals(Convert.ToInt32(searching))
                || NoteDetailsViewModel.referenceData.Value.ToLower().StartsWith(searching.ToLower())
                || NoteDetailsViewModel.user.FirstName.ToLower().StartsWith(searching.ToLower())
                );

                adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);
            }

            adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
            adminQuery = ApplyPagination(adminQuery, PageNumber);

            return View("publishedNotes", adminQuery);

        }




        //AdminDownloadpage
        public ActionResult AdminDownloads(int? userKey,int? buyerKey, int? noteId, int? buyerValue , int? noteValue , int? sellerValue, string SortOrder, string SortBy, int PageNumber = 1)
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                ViewBag.sID = sellerValue;

                var noteList = db.Notes.ToList();
                ViewBag.NoteLTitleList = new SelectList(noteList, "NoteID", "NoteTitle");

                var buyerList = db.Users.Where(m => m.RoleID == 1).ToList();
                ViewBag.BuyerSList = new SelectList(buyerList, "ID", "FirstName");

                var sellerList = db.Users.Where(m => m.RoleID == 1).ToList();
                ViewBag.SellerList = new SelectList(sellerList, "ID", "FirstName");

                List<Note> noteValues = db.Notes.ToList();
                List<ReferenceData> referenceValues = db.ReferenceDatas.ToList();
                List<User> userValues = db.Users.ToList();
                List<DownloadedNote> downloadValues = db.DownloadedNotes.Where(m => m.IsAttachmentDownloaded == true).ToList();
                List<NoteCategory> categoryValues = db.NoteCategories.ToList();

                var adminQuery = from u in userValues
                                join dn in downloadValues on u.ID equals dn.BuyersID
                                join n in noteValues on dn.NoteID equals n.NoteID
                                join cv in categoryValues on n.Category equals cv.ID
                                join rd in referenceValues on n.SellFor equals rd.ID
                                select new NoteDetailsViewModel { note = n, user = u, referenceData = rd, noteCategory = cv, download = dn /*count = p.Select(m => m.NoteID)*/ };


                var sID = Convert.ToInt32(Session["UserId"]);

                if (sellerValue != null)
                {
                    adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.user.ID == sellerValue);

                    adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                    adminQuery = ApplyPagination(adminQuery, PageNumber);
                }

                if (buyerValue != null)
                {
                    adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.user.ID == buyerValue);

                    adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                    adminQuery = ApplyPagination(adminQuery, PageNumber);
                }

                if (noteValue != null)
                {
                    adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.NoteID == noteValue);

                    adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                    adminQuery = ApplyPagination(adminQuery, PageNumber);
                }

                if(noteId != null)
                {
                    var dnote = db.Notes.FirstOrDefault(m => m.NoteID == noteId);
                    if(dnote != null)
                    {
                        string downloadFile = dnote.NoteAttachment;

                        return File(downloadFile, "application/pdf", downloadFile);
                    }
                }

                if (userKey != null)
                {
                    adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.download.SellerID == userKey);

                    adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                    adminQuery = ApplyPagination(adminQuery, PageNumber);
                }
                if (buyerKey != null)
                {
                    adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.download.BuyersID == buyerKey);

                    adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                    adminQuery = ApplyPagination(adminQuery, PageNumber);
                }

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;

                adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);


                return View(adminQuery);
            }

            
        }

        [HttpPost]
        public ActionResult AdminDownloads(string searching, string SortOrder, string SortBy, int PageNumber = 1)
        {
            var sellerList = db.Users.Where(m => m.RoleID == 1).ToList();
            ViewBag.SellerList = new SelectList(sellerList, "ID", "FirstName");

            var noteList = db.Notes.ToList();
            ViewBag.NoteLTitleList = new SelectList(noteList, "NoteID", "NoteTitle");

            var buyerList = db.Users.Where(m => m.RoleID == 1).ToList();
            ViewBag.BuyerSList = new SelectList(buyerList, "ID", "FirstName");


            ViewData["GetReviewNotes"] = searching;

            List<Note> noteValues = db.Notes.ToList();
            List<ReferenceData> referenceValues = db.ReferenceDatas.ToList();
            List<User> userValues = db.Users.ToList();
            List<DownloadedNote> downloadValues = db.DownloadedNotes.Where(m => m.IsAttachmentDownloaded == true).ToList();
            List<NoteCategory> categoryValues = db.NoteCategories.ToList();


            var adminQuery = from u in userValues
                             join dn in downloadValues on u.ID equals dn.BuyersID
                             join n in noteValues on dn.NoteID equals n.NoteID
                             join cv in categoryValues on n.Category equals cv.ID
                             join rd in referenceValues on n.SellFor equals rd.ID
                             select new NoteDetailsViewModel { note = n, user = u, referenceData = rd, noteCategory = cv, download = dn /*count = p.Select(m => m.NoteID)*/ };


            if (!string.IsNullOrEmpty(searching))
            {
                adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.NoteTitle.ToLower().StartsWith(searching.ToLower())
                                            || NoteDetailsViewModel.noteCategory.CategoryName.ToLower().StartsWith(searching.ToLower())
                                            //|| NoteDetailsViewModel.note.SellPrice.Equals(Convert.ToInt32(searching))
                                            || NoteDetailsViewModel.referenceData.Value.ToLower().StartsWith(searching.ToLower())
                                            || NoteDetailsViewModel.user.FirstName.ToLower().StartsWith(searching.ToLower())
                                            );

                adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);
            }



            return View("AdminDownloads", adminQuery);

        }





        //admin Rejected note
        public ActionResult AdminRejectedNote(int? nID, int? noteId, int? sellerValue, string SortOrder, string SortBy, int PageNumber = 1)
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                ViewBag.sID = sellerValue;

                var sellerList = db.Users.Where(m => m.RoleID == 1).ToList();
                ViewBag.SellerList = new SelectList(sellerList, "ID", "FirstName");

                List<Note> noteValues = db.Notes.Where(m => m.Status == 10).ToList();
                List<ReferenceData> referenceValues = db.ReferenceDatas.ToList();
                List<User> userValues = db.Users.ToList();
                List<DownloadedNote> downloadValues = db.DownloadedNotes.Distinct().ToList();
                List<NoteCategory> categoryValues = db.NoteCategories.ToList();


                var adminQuery = from u in userValues
                                 join n in noteValues on u.ID equals n.SellerID
                                 join cv in categoryValues on n.Category equals cv.ID
                                 join rd in referenceValues on n.SellFor equals rd.ID
                                 //join dn in downloadValues on u.ID equals dn.BuyersID 
                                 select new NoteDetailsViewModel { note = n, user = u, referenceData = rd, noteCategory = cv/*, download = dn count = p.Select(m => m.NoteID)*/ };

                var sID = Convert.ToInt32(Session["UserId"]);

                if (sellerValue != null)
                {
                    adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.user.ID == sellerValue);

                    adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                    adminQuery = ApplyPagination(adminQuery, PageNumber);
                }

                if (noteId != null)
                {
                    var dnote = db.Notes.FirstOrDefault(m => m.NoteID == noteId);
                    if (dnote != null)
                    {
                        string downloadFile = dnote.NoteAttachment;

                        return File(downloadFile, "application/pdf", downloadFile);
                    }
                }

                if (nID != null)
                {
                    ApprovedByAdmin(nID);
                }

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;

                adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);


                return View(adminQuery);
            }

        }

        [HttpPost]
        public ActionResult AdminRejectedNote(string searching, string SortOrder, string SortBy, int PageNumber = 1)
        {
            var sellerList = db.Users.Where(m => m.RoleID == 1).ToList();
            ViewBag.SellerList = new SelectList(sellerList, "ID", "FirstName");


            ViewData["GetReviewNotes"] = searching;

            List<Note> noteValues = db.Notes.Where(m => m.Status == 10).ToList();
            List<ReferenceData> referenceValues = db.ReferenceDatas.ToList();
            List<User> userValues = db.Users.ToList();
            List<DownloadedNote> downloadValues = db.DownloadedNotes.ToList();
            List<NoteCategory> categoryValues = db.NoteCategories.ToList();


            var adminQuery = from u in userValues
                             join n in noteValues on u.ID equals n.SellerID
                             join cv in categoryValues on n.Category equals cv.ID
                             join rd in referenceValues on n.SellFor equals rd.ID
                             //join dn in downloadValues on u.ID equals dn.BuyersID
                             select new NoteDetailsViewModel { note = n, user = u, referenceData = rd, noteCategory = cv/*, download = dn count = p.Select(m => m.NoteID)*/ };


            if (!string.IsNullOrEmpty(searching))
            {
                adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.NoteTitle.Contains(searching)
                || NoteDetailsViewModel.noteCategory.CategoryName.Contains(searching)
                || NoteDetailsViewModel.note.SellPrice.Equals(Convert.ToInt32(searching))
                || NoteDetailsViewModel.referenceData.Value.ToLower().StartsWith(searching.ToLower())
                || NoteDetailsViewModel.user.FirstName.ToLower().StartsWith(searching.ToLower())
                );

                adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);
            }



            return View("publishedNotes", adminQuery);

        }




        //User list
        public ActionResult AllMembersList( int? uID, string SortOrder, string SortBy, int PageNumber = 1)
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                List<Note> noteValues = db.Notes.Where(m => m.Status == 10).ToList();
                List<ReferenceData> referenceValues = db.ReferenceDatas.ToList();
                List<User> userValues = db.Users.Where(m => m.IsActive == true).ToList();
                List<DownloadedNote> downloadValues = db.DownloadedNotes.Distinct().ToList();
                List<NoteCategory> categoryValues = db.NoteCategories.ToList();


                var adminQuery = from u in userValues
                                 //join n in noteValues on u.ID equals n.SellerID
                                 //join cv in categoryValues on n.Category equals cv.ID
                                 //join rd in referenceValues on n.SellFor equals rd.ID
                                 //join dn in downloadValues on u.ID equals dn.BuyersID 
                                 select new NoteDetailsViewModel {  user = u/*, note = n, referenceData = rd, noteCategory = cv, download = dn count = p.Select(m => m.NoteID)*/ };

                var sID = Convert.ToInt32(Session["UserId"]);

                if (uID != null)
                {
                    var userEntry = db.Users.FirstOrDefault(m => m.ID == uID);
                    if(userEntry != null)
                    {
                        db.Configuration.ValidateOnSaveEnabled = false;
                        userEntry.IsActive = false;
                        db.Entry(userEntry).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();

                    }
                }

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;

                adminQuery = ApplySortingForMember(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);


                return View(adminQuery);
            }

        }

        [HttpPost]
        public ActionResult AllMembersList(string searching, string SortOrder, string SortBy, int PageNumber = 1)
        {
            
            ViewData["GetReviewNotes"] = searching;

            List<Note> noteValues = db.Notes.Where(m => m.Status == 10).ToList();
            List<ReferenceData> referenceValues = db.ReferenceDatas.ToList();
            List<User> userValues = db.Users.ToList();
            List<DownloadedNote> downloadValues = db.DownloadedNotes.ToList();
            List<NoteCategory> categoryValues = db.NoteCategories.ToList();


            var adminQuery = from u in userValues
                             //join n in noteValues on u.ID equals n.SellerID
                             //join cv in categoryValues on n.Category equals cv.ID
                             //join rd in referenceValues on n.SellFor equals rd.ID
                             //join dn in downloadValues on u.ID equals dn.BuyersID
                             select new NoteDetailsViewModel { user = u/*, note = n, referenceData = rd, noteCategory = cv, download = dn count = p.Select(m => m.NoteID)*/ };


            if (!string.IsNullOrEmpty(searching))
            {
                adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.user.LastName.ToLower().StartsWith(searching.ToLower())
                || NoteDetailsViewModel.user.EmailID.ToLower().StartsWith(searching.ToLower())
                || NoteDetailsViewModel.user.FirstName.ToLower().StartsWith(searching.ToLower())
                );

                adminQuery = ApplySortingForMember(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);
            }

            return View("AllMembersList", adminQuery);

        }



        //Member Details
        public ActionResult MemberDetails(int uid)
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                ViewBag.userIdentify = uid;

                var q1 = db.Users.FirstOrDefault(m => m.ID == uid);

                if (q1 != null)
                {
                    NoteDetailsViewModel model = new NoteDetailsViewModel
                    {
                        user = q1,
                        profileUser = db.Profiles.SingleOrDefault(c => c.SellerID == q1.ID)
                        //noteReview = db.NoteReviews.SingleOrDefault(c => c.NoteID == testNote.NoteID),
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
        public ActionResult _MembersInformation(int uid)
        {
            ViewBag.userIdentify = uid;

            var q1 = db.Users.FirstOrDefault(m => m.ID == uid);

            if (q1 != null)
            {
                NoteDetailsViewModel model = new NoteDetailsViewModel
                {
                    user = q1,
                    profileUser = db.Profiles.SingleOrDefault(c => c.SellerID == q1.ID)
                    //noteReview = db.NoteReviews.SingleOrDefault(c => c.NoteID == testNote.NoteID),
                };
                return PartialView(model);
            }
            else
            {
                return PartialView();
            }

            
        }

        [ChildActionOnly]
        public ActionResult _MembersNoteInformation(int uid, string SortOrder, string SortBy, int PageNumber = 1)
        {
            ViewBag.userIdentify = uid;

            var q1 = db.Users.FirstOrDefault(m => m.ID == uid);

            if(q1 != null)
            {
                List<User> userValues = db.Users.Where(m => m.IsActive == true).ToList();
                List<Profile> profileValues = db.Profiles.ToList();
                List<Note> noteValues = db.Notes.Where(m => m.Status == 9).ToList();
                List<ReferenceData> referenceValues = db.ReferenceDatas.ToList();
                List<NoteCategory> categoryValues = db.NoteCategories.ToList();
                List<DownloadedNote> downloadValues = db.DownloadedNotes.Where(m => m.IsAttachmentDownloaded == true).ToList();

               
               var adminQuery = from u in userValues
                             join n in noteValues on u.ID equals n.SellerID
                             join cv in categoryValues on n.Category equals cv.ID
                             join rd in referenceValues on n.Status equals rd.ID
                             where n.SellerID == uid
                             select new NoteDetailsViewModel { note = n, user = u, referenceData = rd, noteCategory = cv };

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;

                adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);


                return PartialView(adminQuery);

            }

            return PartialView();
        }


        //Manage system confugaration
        public ActionResult ManageSystemConfigurations()
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ManageSystemConfigurations(NoteDetailsViewModel m1)
        {
            if(m1.manageSystemConfiguration.SupportEmail == null)
            {
                ViewBag.supporterr = "Please Enter Support Email Address";
                return View();
            }
            else if(m1.manageSystemConfiguration.SupportPhoneNo == null)
            {
                ViewBag.supporphoneterr = "Please Enter Support Phone No";
                return View();
            }
            else if(m1.manageSystemConfiguration.OtherEmail == null)
            {
                ViewBag.supporotherterr = "Please Enter other Support Email Address";
                return View();
            }
            else
            {
                ManageSystemConfiguration mc = new ManageSystemConfiguration();

                mc.SupportEmail = m1.manageSystemConfiguration.SupportEmail;
                mc.SupportPhoneNo = m1.manageSystemConfiguration.SupportPhoneNo;
                mc.OtherEmail = m1.manageSystemConfiguration.OtherEmail;
                mc.FacebookURL = m1.manageSystemConfiguration.FacebookURL;
                mc.TwitterURL = m1.manageSystemConfiguration.TwitterURL;
                mc.LinkedinURL = m1.manageSystemConfiguration.LinkedinURL;

                if(m1.BookImage == null)
                {
                    mc.DefaultBookImage = "~/Image/search1215941230.png";
                }
                else
                {
                    //Image upload for book image
                    string filename = Path.GetFileNameWithoutExtension(m1.BookImage.FileName);
                    string extension = Path.GetExtension(m1.BookImage.FileName);

                    filename = filename + DateTime.Now.ToString("yymmssfff") + extension;

                    mc.DefaultBookImage = "~/Image/" + filename;

                    filename = Path.Combine(Server.MapPath("~/Image/"), filename);

                    m1.BookImage.SaveAs(filename);
                }
                
                if(m1.UserImage == null)
                {
                    mc.DefaultUserImage = "~/Image/reviewer-1213836826.png";
                }
                else
                {
                    //Image upload for user image
                    string ufilename = Path.GetFileNameWithoutExtension(m1.UserImage.FileName);
                    string uextension = Path.GetExtension(m1.UserImage.FileName);

                    ufilename = ufilename + DateTime.Now.ToString("yymmssfff") + uextension;

                    mc.DefaultUserImage = "~/Image/" + ufilename;

                    ufilename = Path.Combine(Server.MapPath("~/Image/"), ufilename);

                    m1.UserImage.SaveAs(ufilename);
                }
                

                mc.CreatedBy = Convert.ToInt32(Session["UserId"]);
                mc.CreatedDate = DateTime.Now;

                db.ManageSystemConfigurations.Add(mc);
                db.SaveChanges();
            }


              return RedirectToAction("AdminDashboard", "Admin");
        }


        //Add Administrator

        public ActionResult ManageAdministrator(int? uID, string SortOrder, string SortBy, int PageNumber = 1)
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                if (uID != null)
                {
                    var del = db.Users.FirstOrDefault(m => m.ID == uID);
                    if (del != null)
                    {
                        db.Configuration.ValidateOnSaveEnabled = false;

                        del.ModifiedDate = DateTime.Now;
                        del.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                        del.IsActive = false;
                        db.Entry(del).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                    }
                }

                List<User> userValues = db.Users.Where(m => m.RoleID == 2).ToList();
                List<Profile> profileValues = db.Profiles.ToList();

                var adminQuery = from u in userValues
                                 join p in profileValues on u.ID equals p.SellerID
                                 select new NoteDetailsViewModel { user = u, profileUser = p };

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;

                adminQuery = ApplySortingForMember(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);


                return View(adminQuery);
            }

        }

        [HttpPost]
        public ActionResult ManageAdministrator(string searching, string SortOrder, string SortBy, int PageNumber = 1)
        {
            ViewData["GetReviewNotes"] = searching;

            List<User> userValues = db.Users.Where(m => m.RoleID == 2).ToList();
            List<Profile> profileValues = db.Profiles.ToList();

            var adminQuery = from u in userValues
                             join p in profileValues on u.ID equals p.SellerID
                             select new NoteDetailsViewModel { user = u, profileUser = p };

            if (!string.IsNullOrEmpty(searching))
            {
                adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.user.LastName.ToLower().StartsWith(searching.ToLower())
                || NoteDetailsViewModel.user.EmailID.ToLower().StartsWith(searching.ToLower())
                || NoteDetailsViewModel.user.FirstName.ToLower().StartsWith(searching.ToLower())
                );

                adminQuery = ApplySortingForMember(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);
            }

            return View("AllMembersList", adminQuery);
        }

        public ActionResult AddAdministrator(int? userid)
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                var countryCodeList = new List<string>() { "+91", "+1", "+44" };
                ViewBag.CountryCodeList = countryCodeList;

                if (userid == null)
                {
                    return View();
                }
                else
                {
                    NoteDetailsViewModel n1 = new NoteDetailsViewModel();

                    var temp = db.Users.FirstOrDefault(m => m.ID == userid);
                    var temp1 = db.Profiles.FirstOrDefault(m => m.SellerID == userid);
                    n1.user = temp;
                    n1.profileUser = temp1;

                    return View("AddAdministrator", n1);
                }

            }
        }

        [HttpPost]
        public ActionResult AddAdministrator(NoteDetailsViewModel m1)
        {
            var countryCodeList = new List<string>() { "+91", "+1", "+44" };
            ViewBag.CountryCodeList = countryCodeList;

           
            var u1 = db.Users.FirstOrDefault(m => m.EmailID == m1.user.EmailID && m.IsActive == true);
            if (u1 != null)
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                var p1 = db.Profiles.FirstOrDefault(m => m.SellerID == u1.ID);

                u1.FirstName = m1.user.FirstName;
                u1.LastName = m1.user.LastName;
                u1.RoleID = 2;
                p1.CountryCode = m1.profileUser.CountryCode;
                p1.PhoneNo = m1.profileUser.PhoneNo;

                db.Entry(u1).State = System.Data.Entity.EntityState.Modified;
                db.Entry(p1).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();

            }
            else
            {
                ViewBag.NotFound = "EmailId is not present";
                return View();
            }
            

            return RedirectToAction("AdminDashboard", "Admin");
        }



        //Add Category
        public ActionResult ManageCategory(int? cID, string SortOrder, string SortBy, int PageNumber = 1)
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                if (cID != null)
                {
                    var del = db.NoteCategories.FirstOrDefault(m => m.ID == cID);
                    if (del != null)
                    {
                        db.Configuration.ValidateOnSaveEnabled = false;

                        del.ModifiedDate = DateTime.Now;
                        del.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                        del.IsActive = false;
                        db.Entry(del).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                    }
                }
                List<NoteCategory> categoryValues = db.NoteCategories.ToList();
                List<User> userValues = db.Users.ToList();

                var adminQuery = from nc in categoryValues
                                 join u in userValues on nc.CreatedBy equals u.ID
                                 select new NoteDetailsViewModel { user = u, noteCategory = nc };

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;

                adminQuery = ApplySortingCategory(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);


                return View(adminQuery);
            }
        }

        [HttpPost]
        public ActionResult ManageCategory(string searching, string SortOrder, string SortBy, int PageNumber = 1)
        {
            ViewData["GetReviewNotes"] = searching;

            List<NoteCategory> categoryValues = db.NoteCategories.ToList();
            List<User> userValues = db.Users.ToList();

            var adminQuery = from nc in categoryValues
                             join u in userValues on nc.CreatedBy equals u.ID
                             select new NoteDetailsViewModel {user = u, noteCategory = nc };
            if (!string.IsNullOrEmpty(searching))
            {
                adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.noteCategory.CategoryName.ToLower().StartsWith(searching.ToLower())
                ||NoteDetailsViewModel.user.FirstName.ToLower().StartsWith(searching.ToLower())
                );

                adminQuery = ApplySortingCategory(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);
            }

            ViewBag.SortOrder = SortOrder;
            ViewBag.SortBy = SortBy;

            adminQuery = ApplySortingCategory(SortOrder, SortBy, adminQuery);
            adminQuery = ApplyPagination(adminQuery, PageNumber);

            return View("ManageCategory", adminQuery);
        }

        public ActionResult AddCategory(int? categoryid)
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                if (categoryid == null)
                {
                    return View();
                }
                else
                {
                    NoteDetailsViewModel n1 = new NoteDetailsViewModel();
                    var temp = db.NoteCategories.FirstOrDefault(m => m.ID == categoryid);
                    n1.noteCategory = temp;

                    return View("AddCategory", n1);
                }

            }
        }

        [HttpPost]
        public ActionResult AddCategory(NoteDetailsViewModel m1)
        {
            if (string.IsNullOrEmpty(m1.noteCategory.CategoryName))
            {
                ViewBag.nameerr = "Please Enter Category Name";
                return View();
            }
            else if (string.IsNullOrEmpty(m1.noteCategory.CategoryDescription))
            {
                ViewBag.descriptionerr = "Please Enter Description";
                return View();
            }
            else
            { 
                var c1 = db.NoteCategories.FirstOrDefault(m => m.CategoryName == m1.noteCategory.CategoryName);
                if(c1 != null)
                {
                
                    db.Configuration.ValidateOnSaveEnabled = false;

                    c1.CategoryName = m1.noteCategory.CategoryName;
                    c1.CategoryDescription = m1.noteCategory.CategoryDescription;
                    c1.CreatedBy = Convert.ToInt32(Session["UserID"]);
                    c1.CreatedDate = DateTime.Now;
                    c1.IsActive = true;

                    db.Entry(c1).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                
                }
                else
                {
                    NoteCategory newc1 = new NoteCategory();

                    newc1.CategoryName = m1.noteCategory.CategoryName;
                    newc1.CategoryDescription = m1.noteCategory.CategoryDescription;
                    newc1.CreatedBy = Convert.ToInt32(Session["UserID"]);
                    newc1.CreatedDate = DateTime.Now;
                    newc1.IsActive = true;

                    db.NoteCategories.Add(newc1);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ManageCategory", "Admin");
            
        }



        //Add Type
        public ActionResult ManageType(int? tID, string SortOrder, string SortBy, int PageNumber = 1)
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                if (tID != null)
                {
                    var del = db.NoteTypes.FirstOrDefault(m => m.ID == tID);
                    if (del != null)
                    {
                        db.Configuration.ValidateOnSaveEnabled = false;

                        del.ModifiedDate = DateTime.Now;
                        del.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                        del.IsActive = false;
                        db.Entry(del).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                    }
                }
                List<NoteType> typeValues = db.NoteTypes.ToList();
                List<User> userValues = db.Users.ToList();

                var adminQuery = from nt in typeValues
                                 join u in userValues on nt.CreatedBy equals u.ID
                                 select new NoteDetailsViewModel { user = u, typeDetails = nt };

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;

                adminQuery = ApplySortingType(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);


                return View(adminQuery);
            }
        }

        [HttpPost]
        public ActionResult ManageType(string searching, string SortOrder, string SortBy, int PageNumber = 1)
        {
            ViewData["GetReviewNotes"] = searching;

            List<NoteType> typeValues = db.NoteTypes.Where(m => m.IsActive == true).ToList();
            List<User> userValues = db.Users.ToList();

            var adminQuery = from nt in typeValues
                             join u in userValues on nt.CreatedBy equals u.ID
                             select new NoteDetailsViewModel { user = u, typeDetails = nt };


            if (!string.IsNullOrEmpty(searching))
            {
                adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.typeDetails.TypeName.ToLower().StartsWith(searching.ToLower())
                || NoteDetailsViewModel.user.FirstName.ToLower().StartsWith(searching.ToLower())
                );

                adminQuery = ApplySortingType(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);
            }

            ViewBag.SortOrder = SortOrder;
            ViewBag.SortBy = SortBy;

            adminQuery = ApplySortingType(SortOrder, SortBy, adminQuery);
            adminQuery = ApplyPagination(adminQuery, PageNumber);

            return View("ManageType", adminQuery);
        }

        public ActionResult AddType(int? typeid)
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                if (typeid == null)
                {
                    return View();
                }
                else
                {
                    NoteDetailsViewModel n1 = new NoteDetailsViewModel();
                    var temp = db.NoteTypes.FirstOrDefault(m => m.ID == typeid);
                    n1.typeDetails = temp;

                    return View("AddType", n1);
                }

            }
        }

        [HttpPost]
        public ActionResult AddType(NoteDetailsViewModel m1)
        {
            if (string.IsNullOrEmpty(m1.typeDetails.TypeName))
            {
                ViewBag.nameerr = "Please Enter Type Name";
                return View();
            }
            else if (string.IsNullOrEmpty(m1.typeDetails.TypeDescription))
            {
                ViewBag.descriptionerr = "Please Enter Description";
                return View();
            }
            else
            {
                var c1 = db.NoteTypes.FirstOrDefault(m => m.TypeName == m1.typeDetails.TypeName);
                if (c1 != null)
                {
                    db.Configuration.ValidateOnSaveEnabled = false;

                    c1.TypeName = m1.typeDetails.TypeName;
                    c1.TypeDescription = m1.typeDetails.TypeDescription;
                    c1.CreatedBy = Convert.ToInt32(Session["UserID"]);
                    c1.CreatedDate = DateTime.Now;
                    c1.IsActive = true;

                    db.Entry(c1).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
                else
                {
                    NoteType newc1 = new NoteType();

                    newc1.TypeName = m1.typeDetails.TypeName;
                    newc1.TypeDescription = m1.typeDetails.TypeDescription;
                    newc1.CreatedBy = Convert.ToInt32(Session["UserID"]);
                    newc1.CreatedDate = DateTime.Now;
                    newc1.IsActive = true;

                    db.NoteTypes.Add(newc1);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ManageType", "Admin");
        }


        //Add country
        public ActionResult ManageCountry(int? cID, string SortOrder, string SortBy, int PageNumber = 1)
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                if (cID != null)
                {
                    var del = db.NoteCountries.FirstOrDefault(m => m.ID == cID);
                    if (del != null)
                    {
                        db.Configuration.ValidateOnSaveEnabled = false;

                        del.ModifiedDate = DateTime.Now;
                        del.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                        del.IsActive = false;
                        db.Entry(del).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                    }
                }
                List<NoteCountry> countryValues = db.NoteCountries.ToList();
                List<User> userValues = db.Users.ToList();

                var adminQuery = from nc in countryValues
                                 join u in userValues on nc.CreatedBy equals u.ID
                                 select new NoteDetailsViewModel { user = u, countryDetails = nc };

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;

                adminQuery = ApplySortingCountry(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);


                return View(adminQuery);
            }
        }

        [HttpPost]
        public ActionResult ManageCountry(string searching, string SortOrder, string SortBy, int PageNumber = 1)
        {
            ViewData["GetReviewNotes"] = searching;

            List<NoteCountry> countryValues = db.NoteCountries.Where(m => m.IsActive == true).ToList();
            List<User> userValues = db.Users.ToList();

            var adminQuery = from nc in countryValues
                             join u in userValues on nc.CreatedBy equals u.ID
                             select new NoteDetailsViewModel { user = u, countryDetails = nc };



            if (!string.IsNullOrEmpty(searching))
            {
                adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.countryDetails.CountryName.ToLower().StartsWith(searching.ToLower())
                || NoteDetailsViewModel.user.FirstName.ToLower().StartsWith(searching.ToLower())
                );

                adminQuery = ApplySortingCountry(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);
            }

            ViewBag.SortOrder = SortOrder;
            ViewBag.SortBy = SortBy;

            adminQuery = ApplySortingCountry(SortOrder, SortBy, adminQuery);
            adminQuery = ApplyPagination(adminQuery, PageNumber);

            return View("ManageCountry", adminQuery);
        }

        public ActionResult AddCountry(int? countryid)
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                if (countryid == null)
                {
                    return View();
                }
                else
                {
                    NoteDetailsViewModel n1 = new NoteDetailsViewModel();
                    var temp = db.NoteCountries.FirstOrDefault(m => m.ID == countryid);
                    n1.countryDetails = temp;

                    return View("AddCountry", n1);
                }

            }
        }

        [HttpPost]
        public ActionResult AddCountry(NoteDetailsViewModel m1)
        {
            if (string.IsNullOrEmpty(m1.countryDetails.CountryName))
            {
                ViewBag.nameerr = "Please Enter Country Name";
                return View();
            }
            else if (m1.countryDetails.CountryCode == null)
            {
                ViewBag.descriptionerr = "Please Enter Country Code";
                return View();
            }
            else
            {
                var c1 = db.NoteCountries.FirstOrDefault(m => m.CountryName == m1.countryDetails.CountryName);
                if (c1 != null)
                {
                    db.Configuration.ValidateOnSaveEnabled = false;

                    c1.CountryName = m1.countryDetails.CountryName;
                    c1.CountryCode = m1.countryDetails.CountryCode;
                    c1.CreatedBy = Convert.ToInt32(Session["UserID"]);
                    c1.CreatedDate = DateTime.Now;
                    c1.IsActive = true;

                    db.Entry(c1).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
                else
                {
                    NoteCountry newc1 = new NoteCountry();

                    newc1.CountryName = m1.countryDetails.CountryName;
                    newc1.CountryCode = m1.countryDetails.CountryCode;
                    newc1.CreatedBy = Convert.ToInt32(Session["UserID"]);
                    newc1.CreatedDate = DateTime.Now;
                    newc1.IsActive = true;

                    db.NoteCountries.Add(newc1);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("ManageCountry", "Admin");
        }


        //Admin Spam Report
        public ActionResult AdminSpamReport(int? reviewId, int? noteId, string SortOrder, string SortBy, int PageNumber = 1)
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                var del = db.NoteReviews.FirstOrDefault(m => m.ID == reviewId);
                if (del != null)
                {
                    var tempreject = db.RejectedNotes.FirstOrDefault(m => m.AgainstDownloadsID == del.AgainstDownloadsID);
                    db.RejectedNotes.Remove(tempreject);
                    db.NoteReviews.Remove(del);
                    db.SaveChanges();
                }

                List<Note> noteValues = db.Notes.Where(m => m.Status == 9).ToList();
                List<User> userValues = db.Users.ToList();
                List<NoteReview> reviewValues = db.NoteReviews.Where( m=>m.Inappropriate == true).ToList();
                List<NoteCategory> categoryValues = db.NoteCategories.ToList();
                List<RejectedNote> rejectValues = db.RejectedNotes.ToList();

                var adminQuery = from u in userValues
                                 join r in reviewValues on u.ID equals r.ModifiedBy
                                 join n in noteValues on r.NoteID equals n.NoteID
                                 join cv in categoryValues on n.Category equals cv.ID
                                 join rn in rejectValues on u.ID equals rn.BuyersID
                                 //join dn in downloadValues on u.ID equals dn.BuyersID 
                                 select new NoteDetailsViewModel { note = n, user = u, noteCategory = cv, noteReview = r,rejectedNote = rn };
                if (noteId != null)
                {
                    var dnote = db.Notes.FirstOrDefault(m => m.NoteID == noteId);
                    if (dnote != null)
                    {
                        string downloadFile = dnote.NoteAttachment;

                        return File(downloadFile, "application/pdf", downloadFile);
                    }
                }

                ViewBag.SortOrder = SortOrder;
                ViewBag.SortBy = SortBy;

                adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);


                return View(adminQuery);
            }
        }

        [HttpPost]
        public ActionResult AdminSpamReport(string searching, string SortOrder, string SortBy, int PageNumber = 1)
        {
            List<Note> noteValues = db.Notes.Where(m => m.Status == 9).ToList();
            List<User> userValues = db.Users.ToList();
            List<NoteReview> reviewValues = db.NoteReviews.Where( m=> m.Inappropriate == true).ToList();
            List<NoteCategory> categoryValues = db.NoteCategories.ToList();
            List<RejectedNote> rejectValues = db.RejectedNotes.ToList();

            var adminQuery = from u in userValues
                             join r in reviewValues on u.ID equals r.ModifiedBy
                             join n in noteValues on r.NoteID equals n.NoteID
                             join cv in categoryValues on n.Category equals cv.ID
                             join rn in rejectValues on u.ID equals rn.BuyersID
                             select new NoteDetailsViewModel { note = n, user = u, noteCategory = cv, noteReview = r, rejectedNote = rn };


            if (!string.IsNullOrEmpty(searching))
            {
                adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.NoteTitle.Contains(searching)
                || NoteDetailsViewModel.noteCategory.CategoryName.Contains(searching)
                || NoteDetailsViewModel.user.FirstName.ToLower().StartsWith(searching.ToLower())
                );

                adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);
            }

            adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
            adminQuery = ApplyPagination(adminQuery, PageNumber);

            return View("AdminSpamReport", adminQuery);
        }


        //Admin Profile update
        public ActionResult AdminProfileUpdate()
        {
            if (Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                var countryCodeList = new List<string>() { "+91", "+1", "+44" };
                ViewBag.CountryCodeList = countryCodeList;

                ViewData["userEmail"] = Session["UserEmailID"].ToString();

                var aid = Convert.ToInt32(Session["UserId"]);
                var admintemp = db.Users.FirstOrDefault(m => m.ID == aid);
                var adminprofile = db.Profiles.FirstOrDefault(m => m.SellerID == aid);

                if(admintemp != null)
                {
                    if(adminprofile != null)
                    {
                        if(adminprofile.PhoneNo != null)
                        {
                            NoteDetailsViewModel n1 = new NoteDetailsViewModel();

                            n1.user = admintemp;
                            n1.profileUser = adminprofile;

                            return View("AdminProfileUpdate", n1);
                        }
                        else
                        {
                            ViewData["FirstName"] = admintemp.FirstName;
                            ViewData["LastNme"] = admintemp.LastName;
                            ViewData["Email"] = Session["UserEmailId"].ToString();
                            return View();
                        }
                        
                    }
                    else
                    {
                        ViewData["FirstName"] = admintemp.FirstName;
                        ViewData["LastNme"] = admintemp.LastName;
                        ViewData["Email"] = Session["UserEmailId"].ToString();
                        return View();
                    }
                }
                else
                {
                    ViewData["FirstName"] = admintemp.FirstName;
                    ViewData["LastNme"] = admintemp.LastName;
                    ViewData["Email"] = Session["UserEmailId"].ToString();
                    return View();
                }
            }
        }

        [HttpPost]
        public ActionResult AdminProfileUpdate(NoteDetailsViewModel m1)
        {
            var countryCodeList = new List<string>() { "+91", "+1", "+44" };
            ViewBag.CountryCodeList = countryCodeList;

            if (m1.profileUser.SecondaryEmailID == null)
            {
                ViewBag.secondemailerr = "Please Enter Secondaey Email ID";
                return View();
            }
            else
            {
                var aemail = Session["UserEmailId"].ToString();

                var u1 = db.Users.FirstOrDefault(m => m.EmailID == aemail);
                if (u1 != null)
                {
                    db.Configuration.ValidateOnSaveEnabled = false;
                    var p1 = db.Profiles.FirstOrDefault(m => m.SellerID == u1.ID);

                    u1.FirstName = m1.user.FirstName;
                    u1.LastName = m1.user.LastName;
                    p1.SecondaryEmailID = m1.profileUser.SecondaryEmailID;
                    p1.CountryCode = m1.profileUser.CountryCode;
                    p1.PhoneNo = m1.profileUser.PhoneNo;

                    if (m1.UserImage == null)
                    {
                        p1.ProfilePhoto = "~/Image/reviewer-1213836826.png";
                    }
                    else
                    {
                        //Image upload for user image
                        string ufilename = Path.GetFileNameWithoutExtension(m1.UserImage.FileName);
                        string uextension = Path.GetExtension(m1.UserImage.FileName);

                        ufilename = ufilename + DateTime.Now.ToString("yymmssfff") + uextension;

                        p1.ProfilePhoto = "~/Image/" + ufilename;

                        ufilename = Path.Combine(Server.MapPath("~/Image/"), ufilename);

                        m1.UserImage.SaveAs(ufilename);
                    }
                    p1.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    p1.ModifiedDate = DateTime.Now;


                    db.Entry(u1).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(p1).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                }
                else
                {
                    ViewBag.NotFound = "EmailId is not present";
                }
            }
            return RedirectToAction("AdminDashboard", "Admin");

        }





        [NonAction]
        public void EmailForUnpublishNote(int? unpublishNote, string sEmail)
        {
            
            var fromEmail = new MailAddress("3999vachauhan@gmail.com", "V@chauhan3999");
            var toEmail = new MailAddress(sEmail);
            var fromEmailPassword = "nnumqfegkqbdecgs";

            string subject = " Sorry ! We need to remove your notes from our portal";

            string body = "Hello " + db.Users.FirstOrDefault(m => m.EmailID == sEmail).FirstName + ", <br/> We would like to inform you that " + db.Notes.FirstOrDefault(m => m.NoteID == unpublishNote).NoteTitle + "  has been removed from the portal. Please find our remarks as below-"+ db.Notes.FirstOrDefault(m => m.NoteID == unpublishNote).AdminRemarks+" <br/> <br/> Regards,<br/> Notes Marketlace";
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
        

        //Functions
        public IEnumerable<NoteDetailsViewModel> ApplySorting(string SortOrder, string SortBy, IEnumerable<NoteDetailsViewModel> query1)
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
                case "First Name":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.FirstName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Last Name":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.LastName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.LastName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.LastName).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Reported By":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.FirstName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Date Edited":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.noteReview.ModifiedDate).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.noteReview.ModifiedDate).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.noteReview.ModifiedDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Seller":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.FirstName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Published Date":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.note.PublishedDate).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.note.PublishedDate).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.note.PublishedDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Publisher":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.FirstName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Date Added":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.note.PublishedDate).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.note.PublishedDate).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.note.PublishedDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Downloaded Date/Time":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.download.AttachmentDownloadedDate).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.download.AttachmentDownloadedDate).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.download.AttachmentDownloadedDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Rejected By":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.FirstName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
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


        public IEnumerable<NoteDetailsViewModel> ApplySortingForMember(string SortOrder, string SortBy, IEnumerable<NoteDetailsViewModel> query1)
        {
            switch (SortBy)
            {
               
                case "First Name":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.FirstName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Last Name":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.LastName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.LastName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.LastName).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Email":
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
                case "Joining Date":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.CreatedDate).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.CreatedDate).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.CreatedDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Date Added":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.CreatedDate).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.CreatedDate).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.CreatedDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }

                default:
                    {
                        query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                        break;
                    }
            }
            return query1;
        }

        public IEnumerable<NoteDetailsViewModel> ApplySortingCategory(string SortOrder, string SortBy, IEnumerable<NoteDetailsViewModel> query1)
        {
            switch (SortBy)
            {
                
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
                
                case "First Name":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.FirstName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Last Name":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.LastName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.LastName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.LastName).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Date Added":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.noteCategory.CreatedDate).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.noteCategory.CreatedDate).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.noteCategory.CreatedDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Added By":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.FirstName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                        }
                        break;
                    }

                default:
                    {
                        query1 = query1.OrderBy(x => x.noteCategory.CreatedDate).ToList();
                        break;
                    }
            }
            return query1;
        }

        public IEnumerable<NoteDetailsViewModel> ApplySortingType(string SortOrder, string SortBy, IEnumerable<NoteDetailsViewModel> query1)
        {
            switch (SortBy)
            {

                case "Type":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.typeDetails.TypeName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.typeDetails.TypeName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.typeDetails.TypeName).ToList();
                                    break;
                                }
                        }
                        break;
                    }

                case "First Name":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.FirstName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Last Name":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.LastName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.LastName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.LastName).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Date Added":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.typeDetails.CreatedDate).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.typeDetails.CreatedDate).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.typeDetails.CreatedDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Added By":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.FirstName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                        }
                        break;
                    }

                default:
                    {
                        query1 = query1.OrderByDescending(x => x.typeDetails.CreatedDate).ToList();
                        break;
                    }
            }
            return query1;
        }

        public IEnumerable<NoteDetailsViewModel> ApplySortingCountry(string SortOrder, string SortBy, IEnumerable<NoteDetailsViewModel> query1)
        {
            switch (SortBy)
            {

                case "Country Name":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.countryDetails.CountryName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.countryDetails.CountryName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.countryDetails.CountryName).ToList();
                                    break;
                                }
                        }
                        break;
                    }

                case "First Name":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.FirstName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Last Name":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.LastName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.LastName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.LastName).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Date Added":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.countryDetails.CreatedDate).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.countryDetails.CreatedDate).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.countryDetails.CreatedDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Added By":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.user.FirstName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.user.FirstName).ToList();
                                    break;
                                }
                        }
                        break;
                    }

                default:
                    {
                        query1 = query1.OrderByDescending(x => x.countryDetails.CreatedDate).ToList();
                        break;
                    }
            }
            return query1;
        }


        public IEnumerable<NoteDetailsViewModel> ApplyPagination(IEnumerable<NoteDetailsViewModel> adminQuery,int PageNumber = 1)
        {
            ViewBag.PageTotal = Math.Ceiling(adminQuery.Count() / 5.0);

            ViewBag.TotalRecord = adminQuery.Count();
            ViewBag.PageNumber = PageNumber;

            //int p = Convert.ToInt32(displayValue);
            adminQuery = adminQuery.Skip((PageNumber - 1) * 5).Take(5).ToList();

            return adminQuery;
        }

        public void ApprovedByAdmin(int? nID)
        {
            var sID = Convert.ToInt32(Session["UserId"]);

            if (nID != null)
            {
                var temp = db.Notes.FirstOrDefault(m => m.NoteID == nID);

                if (temp != null)
                {
                    db.Configuration.ValidateOnSaveEnabled = false;
                    temp.Status = 9;
                    db.Entry(temp).State = System.Data.Entity.EntityState.Modified;

                    temp.ApprovedDate = DateTime.Now;
                    db.Entry(temp).State = System.Data.Entity.EntityState.Modified;

                    temp.ApprovedBy = sID;
                    db.Entry(temp).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
            }
            //var check = Request.Url.Segments[2].ToString();
            //if(check == "ApprovedByAdmin")
            //{
            //    return RedirectToAction("NotesUnderReview");
            //}
            //if(check == "AdminRejectedNote")
            //{
            //    return RedirectToAction("AdminRejectedNote");
            //}
            //else
            //{
            //    return RedirectToAction("AdminRejectedNote");
            //}

        }

        public ActionResult RejectedByAdmin(int? nID, string rejectRemrks)
        {
            var sID = Convert.ToInt32(Session["UserId"]);

            if (nID != null)
            {
                var temp = db.Notes.FirstOrDefault(m => m.NoteID == nID);

                if (temp != null)
                {
                    db.Configuration.ValidateOnSaveEnabled = false;
                    temp.Status = 10;
                    db.Entry(temp).State = System.Data.Entity.EntityState.Modified;

                    temp.AdminRemarks = rejectRemrks;
                    db.Entry(temp).State = System.Data.Entity.EntityState.Modified;

                    temp.ActionedBy = sID;
                    db.Entry(temp).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
            }

            return RedirectToAction("NotesUnderReview");

        }

        public ActionResult InReviewByAdmin(int? nID)
        {
            var sID = Convert.ToInt32(Session["UserId"]);

            if (nID != null)
            {
                var temp = db.Notes.FirstOrDefault(m => m.NoteID == nID);

                if (temp != null)
                {

                    db.Configuration.ValidateOnSaveEnabled = false;

                    temp.Status = 8;
                    db.Entry(temp).State = System.Data.Entity.EntityState.Modified;

                    temp.ActionedBy = sID;
                    db.Entry(temp).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
            }

            return RedirectToAction("NotesUnderReview");
        }

       
    }
}