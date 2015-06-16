using Newtonsoft.Json;
using ReservationCalendar.DAL;
using ReservationCalendar.Models;
using ReservationCalendar.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;

namespace ReservationCalendar.API
{
    public class ReservationBookAbsApiController : ApiController
    {
        private ReservationCalendarContext db = new ReservationCalendarContext();

        // GET: api/ReservationBookAbsApi/GetReservationBookAbs/1
        [ResponseType(typeof(ReservationBookAbs))]
        public async Task<IHttpActionResult> GetReservationBookAbs(int id, long startTime, long endTime)
        {
            ReservationBook rBook = await db.ReservationBooks.Where(r => r.ID == id).SingleOrDefaultAsync<ReservationBook>();
            TimePeriod timePeriod = new TimePeriod { unitsAsDays = true, startTime = startTime, endTime = endTime };
            ReservationBookAbs rBookAbs = new ReservationBookAbs(rBook, timePeriod, false, false);

            if (rBookAbs == null)
            {
                return NotFound();
            }

            return Ok(rBookAbs);
        }       
    }
}
