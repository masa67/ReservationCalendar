using Newtonsoft.Json;
using ReservationCalendar.DAL;
using ReservationCalendar.Helpers;
using ReservationCalendar.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ReservationCalendar.Controllers
{
    public class ReservationBookDTOController : Controller
    {
        private ReservationCalendarContext db = new ReservationCalendarContext();

        public ReservationBookDTOController()
        {
        }

        // GET: ReservationBookDTO
        public ActionResult Index()
        {
            List<ReservationBook> rBooks = db.ReservationBooks
                .Include(r => r.CalendarBookAllocations)
                .ToList<ReservationBook>();
            
            List<ReservationBookDTO> rBooksDTO = new List<ReservationBookDTO>();

            foreach (ReservationBook rBook in rBooks)
            {
                rBooksDTO.Add(new ReservationBookDTO(rBook, null, true, true, true));
            }

            return View(rBooksDTO);
        }

        // GET: ReservationBookDTO/Details/1
        public ActionResult Details(int? id)
        {
            // IQueryable<ReservationBook> rBookQuery =
            //        from rbook in db.ReservationBooks
            //        select rbook;
           
            ReservationBookDTO rBookDTO = null;
         
            var rBookQuery =
               db.ReservationBooks
                   .Include(r => r.CalendarBookAllocations)
                   .Where(r => r.ID == (id + 1));

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);            
            }

            foreach (ReservationBook rBook in rBookQuery)
            {
                rBookDTO = new ReservationBookDTO(rBook, null, true, true, true);
            }

            if (rBookDTO == null)
            {
                return HttpNotFound();
            }

            return View(rBookDTO);
        }       
    }
}