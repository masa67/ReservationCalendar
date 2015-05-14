using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ReservationCalendar.DAL;
using ReservationCalendar.Models;

namespace ReservationCalendar.Controllers
{
    public class ReservationBookController : Controller
    {
        private ReservationCalendarContext db = new ReservationCalendarContext();

        // GET: ReservationBook
        public ActionResult Index()
        {
            return View(db.ReservationBooks.ToList());
        }

        // GET: ReservationBook/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReservationBook reservationBook = db.ReservationBooks.Find(id);
            if (reservationBook == null)
            {
                return HttpNotFound();
            }
          
            return View(reservationBook);
        }

        // GET: ReservationBook/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReservationBook/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Description,StartTime,EndTime")] ReservationBook reservationBook)
        {
            if (ModelState.IsValid)
            {
                db.ReservationBooks.Add(reservationBook);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(reservationBook);
        }

        // GET: ReservationBook/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReservationBook reservationBook = db.ReservationBooks.Find(id);
            if (reservationBook == null)
            {
                return HttpNotFound();
            }
            return View(reservationBook);
        }

        // POST: ReservationBook/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Description,StartTime,EndTime")] ReservationBook reservationBook)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservationBook).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(reservationBook);
        }

        // GET: ReservationBook/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReservationBook reservationBook = db.ReservationBooks.Find(id);
            if (reservationBook == null)
            {
                return HttpNotFound();
            }
            return View(reservationBook);
        }

        // POST: ReservationBook/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReservationBook reservationBook = db.ReservationBooks.Find(id);
            db.ReservationBooks.Remove(reservationBook);
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
