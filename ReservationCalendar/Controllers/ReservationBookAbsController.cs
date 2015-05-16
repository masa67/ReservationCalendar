using Newtonsoft.Json;
using ReservationCalendar.DAL;
using ReservationCalendar.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

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
        public ActionResult Details(int? id, string data)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);            
            }
            if (id >= rBooksAbs.Count)
            {
                return HttpNotFound();
            }

            if (!string.IsNullOrEmpty(data) && data.Equals("true"))
            {
                return Content(JsonConvert.SerializeObject(
                    rBooksAbs[id ?? default(int)],
                    Formatting.Indented,
                    new JsonSerializerSettings {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    }));
            } else {
                return View(rBooksAbs[id ?? default(int)]);
            }
        }
    }
}