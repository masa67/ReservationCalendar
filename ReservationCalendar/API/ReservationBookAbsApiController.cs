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

namespace ReservationCalendar.API
{
    public class ReservationBookAbsApiController : ApiController
    {
        private ReservationCalendarContext db = new ReservationCalendarContext();

        // GET: api/ReservationBookAbsApi/GetReservationBookAbs/1
        [ResponseType(typeof(ReservationBookAbs))]
        public async Task<OperationStatus> GetReservationBookAbs(int id)
        {
            ReservationBook rBook = await db.ReservationBooks.Where(r => r.ID == id).SingleOrDefaultAsync<ReservationBook>();
            ReservationBookAbs rBookAbs = new ReservationBookAbs(rBook, null, false, false, false);
            OperationStatus ret;

            if (rBookAbs != null)
            {
                ret = new OperationStatus { Status = true, Data = rBookAbs };
            }
            else
            {
                ret = new OperationStatus { Status = false, Message = "Not found" };
            }

            return ret;
        }       

        // GET: api/ReservationBookAbsApi/GetReservationBookAbs/1?startTime=<long>&endTime=<long>
        [ResponseType(typeof(ReservationBookAbs))]
        public async Task<OperationStatus> GetReservationBookAbs(int id, long startTime, long endTime)
        {
            ReservationBook rBook = await db.ReservationBooks
                .Include(r => r.CalendarBookAllocations)
                .Where(r => r.ID == id).SingleOrDefaultAsync<ReservationBook>();
            TimePeriod timePeriod = new TimePeriod { unitsAsDays = true, startTime = startTime, endTime = endTime };
            ReservationBookAbs rBookAbs = new ReservationBookAbs(rBook, timePeriod, true, false, false);
            OperationStatus ret;

            if (rBookAbs != null)
            {
                ret = new OperationStatus { Status = true, Data = rBookAbs };
            }
            else
            {
                ret = new OperationStatus { Status = false, Message = "Not found" };
            }

            return ret;
        }       
    }
}
