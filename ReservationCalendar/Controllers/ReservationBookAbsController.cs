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
        public ActionResult Details(int? id, string data)
        {
            // IQueryable<ReservationBook> rBookQuery =
            //        from rbook in db.ReservationBooks
            //        select rbook;
            ReservationBookAbs rBookAbs = null;
            Boolean retJSON;

            var rBookQuery =
                db.ReservationBooks.
                Where("ID = @0", id + 1);

            long startTime = TimeHelper.DateTimeToUTCTimeStamp(new DateTime(2015, 5, 11), false);
            long endTime = TimeHelper.DateTimeToUTCTimeStamp(new DateTime(2015, 5, 15), false);
            TimePeriod timePeriod = new TimePeriod { unitsAsDays = true, startTime = startTime, endTime = endTime };

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);            
            }

            retJSON = !string.IsNullOrEmpty(data) && data.Equals("true");

            foreach (ReservationBook rBook in rBookQuery)
            {
                if (retJSON) {
                    rBookAbs = new ReservationBookAbs(rBook, timePeriod, false, false);
                } else {
                    rBookAbs = new ReservationBookAbs(rBook, timePeriod, true, true);
                }
            }

            if (rBookAbs == null)
            {
                return HttpNotFound();
            }

            if (retJSON)
            {
                return Content(JsonConvert.SerializeObject(
                    rBookAbs,
                    Formatting.Indented,
                    new JsonSerializerSettings {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    }));
            } else {
                return View(rBookAbs);
            }
        }
    }
}