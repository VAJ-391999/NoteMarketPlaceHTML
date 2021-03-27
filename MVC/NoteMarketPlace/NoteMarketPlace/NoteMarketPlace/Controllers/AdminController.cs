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
        public ActionResult AdminDashboard(int? ivalue,string selectMonth, string SortOrder, string SortBy, int PageNumber = 1)
        {
            if(Session["UserEmailID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                //ViewBag.month = DateTime.Now.ToString("MM/dd/yyyy HH':'mm");

                var monthList = new List<string>() { "January", "Feb", "March", "April", "May", "June", "July", "Aug", "Sep", "Oct", "Nov", "Dec" };
                ViewBag.MonthList = monthList;

                //var myList = new List<KeyValuePair<string, int>>();
                //myList.Add(new KeyValuePair<string, int>("Jan", 1));
                //myList.Add(new KeyValuePair<string, int>("Feb", 2));
                //myList.Add(new KeyValuePair<string, int>("Mar", 3));
                //myList.Add(new KeyValuePair<string, int>("Apr", 4));
                //myList.Add(new KeyValuePair<string, int>("May", 5));
                //myList.Add(new KeyValuePair<string, int>("Jun", 6));
                //myList.Add(new KeyValuePair<string, int>("Jul", 7));
                //myList.Add(new KeyValuePair<string, int>("Aug", 8));
                //myList.Add(new KeyValuePair<string, int>("Sep", 9));
                //myList.Add(new KeyValuePair<string, int>("Oct", 10));
                //myList.Add(new KeyValuePair<string, int>("Nov", 11));
                //myList.Add(new KeyValuePair<string, int>("Dec", 12));

                //ViewBag.MonthList = new SelectList(myList.Select(x => new { Value = x.Key, Text = x.Value}), "Value", "Text");


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

                //var adminQuery = from n in noteValues
                //                 join u in userValues on n.SellerID equals u.ID
                //                 join rd in referenceValues on n.SellFor equals rd.ID
                //                 join dn in downloadValues on n.NoteID equals dn.NoteID 
                //                 join cv in categoryValues on n.Category equals cv.ID
                //                 //group dn by dn.NoteID into p
                //                 select new NoteDetailsViewModel { note = n, user = u, referenceData = rd , noteCategory = cv /*count = p.Select(m => m.NoteID)*/ };

                if (!string.IsNullOrEmpty(selectMonth))
                {
                    adminQuery = from u in userValues
                                 join n in noteValues on u.ID equals n.SellerID
                                 join cv in categoryValues on n.Category equals cv.ID
                                 join rd in referenceValues on n.SellFor equals rd.ID
                                 where DateTime.ParseExact(n.PublishedDate.ToString(),"dd-MM-yyyy hh'.'mm'.'ss tt", null).ToString("MMMM") == selectMonth 
                                 select new NoteDetailsViewModel { note = n, user = u, referenceData = rd, noteCategory = cv/*, download = dn count = p.Select(m => m.NoteID)*/ };
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
        public async Task<ActionResult> AdminDashboard(string searching,string SortOrder, string SortBy, int PageNumber = 1)
        {

            ViewData["GetpublishedNotes"] = searching;

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

            //var adminQuery = from n in noteValues
            //                    join u in userValues on n.SellerID equals u.ID
            //                    join rd in referenceValues on n.SellFor equals rd.ID
            //                    join dn in downloadValues on n.NoteID equals dn.NoteID
            //                    join cv in categoryValues on n.Category equals cv.ID
            //                 //group dn by dn.NoteID into p
            //                 select new NoteDetailsViewModel { note = n, user = u, referenceData = rd, noteCategory = cv /*count = p.Select(m => m.NoteID)*/ };

            if (!string.IsNullOrEmpty(searching))
            {
                adminQuery = adminQuery.Where(NoteDetailsViewModel => NoteDetailsViewModel.note.NoteTitle.Contains(searching)
                //|| NoteDetailsViewModel.noteCategory.CategoryName.Contains(searching)
                );

                adminQuery = ApplySorting(SortOrder, SortBy, adminQuery);
                adminQuery = ApplyPagination(adminQuery, PageNumber);
            }

            return View("AdminDashboard", adminQuery);
            


        }

        public ActionResult NotesUnderReview()
        {
            return View();
        }

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
                default:
                    {
                        query1 = query1.OrderBy(x => x.note.NoteTitle).ToList();
                        break;
                    }
            }
            return query1;
        }

        public IEnumerable<NoteDetailsViewModel> ApplyPagination(IEnumerable<NoteDetailsViewModel> adminQuery,int PageNumber = 1)
        {
            ViewBag.PageTotal = Math.Ceiling(adminQuery.Count() / 3.0);

            ViewBag.TotalRecord = adminQuery.Count();
            ViewBag.PageNumber = PageNumber;

            //int p = Convert.ToInt32(displayValue);
            adminQuery = adminQuery.Skip((PageNumber - 1) * 3).Take(3).ToList();

            return adminQuery;
        }
    }
}