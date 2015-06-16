using Newtonsoft.Json;
using ReservationCalendar.DAL;
using ReservationCalendar.Helpers;
using ReservationCalendar.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ReservationCalendar.Controllers
{
    public class ReservationBookAbsController : Controller
    {
        private ReservationCalendarContext db = new ReservationCalendarContext();

        public ReservationBookAbsController()
        {
        }

        // GET: ReservationBookAbs
        public ActionResult Index()
        {
            IQueryable<ReservationBook> rBookQuery =
                from rbook in db.ReservationBooks
                select rbook;
            List<ReservationBookAbs> rBooksAbs = new List<ReservationBookAbs>();

            foreach (ReservationBook rBook in rBookQuery)
            {
                rBooksAbs.Add(new ReservationBookAbs(rBook, null, true, true));
            }

            return View(rBooksAbs);
        }

        // GET: ReservationBookAbs/Details/1
        public ActionResult Details(int? id)
        {
            // IQueryable<ReservationBook> rBookQuery =
            //        from rbook in db.ReservationBooks
            //        select rbook;
           
            ReservationBookAbs rBookAbs = null;

            var rBookQuery =
               db.ReservationBooks.
               Where(r => r.ID == (id + 1));
               // Where("ID = @0", id + 1); - works as well

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);            
            }

            foreach (ReservationBook rBook in rBookQuery)
            {
                rBookAbs = new ReservationBookAbs(rBook, null, true, true);
            }

            if (rBookAbs == null)
            {
                return HttpNotFound();
            }

            return View(rBookAbs);
        }       
    }
}