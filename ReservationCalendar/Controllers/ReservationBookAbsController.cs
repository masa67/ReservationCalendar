using ReservationCalendar.DAL;
using ReservationCalendar.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ReservationCalendar.Controllers
{
    public class ReservationBookAbsController : Controller
    {
        private ReservationCalendarContext db = new ReservationCalendarContext();
        private List<ReservationBookAbs> rBooksAbs;

        public ReservationBookAbsController()
        {
            ICollection<ReservationBook> rBooks = db.ReservationBooks.ToList();
            rBooksAbs = new List<ReservationBookAbs>();


            foreach (ReservationBook rBook in rBooks)
            {
                rBooksAbs.Add(new ReservationBookAbs(rBook));
            }
        }

        // GET: ReservationBookAbs
        public ActionResult Index()
        {
            return View(rBooksAbs);
        }

        // GET: ReservationBookAbs/Details/1
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);            
            }
            if (id >= rBooksAbs.Count)
            {
                return HttpNotFound();
            }        

            return View(rBooksAbs[id ?? default(int)]);
        }
    }
}