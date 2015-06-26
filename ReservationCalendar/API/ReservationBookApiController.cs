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
    public class ReservationBookApiController : ApiController
    {
        private ReservationCalendarContext db = new ReservationCalendarContext();

        // GET: api/ReservationBookApi/GetReservationBook/1
        [ResponseType(typeof(ReservationBookDTO))]
        public async Task<OperationStatus> GetReservationBook(int id)
        {
            ReservationBook rBook = await db.ReservationBooks.Where(r => r.ID == id).SingleOrDefaultAsync<ReservationBook>();
            ReservationBookDTO rBookDTO = new ReservationBookDTO(rBook, null, false, false, false);
            OperationStatus ret;

            if (rBookDTO != null)
            {
                ret = new OperationStatus { Status = true, Data = rBookDTO };
            }
            else
            {
                ret = new OperationStatus { Status = false, Message = "Not found" };
            }

            return ret;
        }       

        // GET: api/ReservationBookApi/GetReservationBook/1?startTime=<long>&endTime=<long>
        [ResponseType(typeof(ReservationBookDTO))]
        public async Task<OperationStatus> GetReservationBook(int id, long startTime, long endTime)
        {
            ReservationBook rBook = await db.ReservationBooks
                .Include(r => r.CalendarBookAllocations)
                .Where(r => r.ID == id).SingleOrDefaultAsync<ReservationBook>();
            TimePeriod timePeriod = new TimePeriod { unitsAsDays = true, startTime = startTime, endTime = endTime };
            ReservationBookDTO rBookDTO = new ReservationBookDTO(rBook, timePeriod, true, false, false);
            OperationStatus ret;

            if (rBookDTO != null)
            {
                ret = new OperationStatus { Status = true, Data = rBookDTO };
            }
            else
            {
                ret = new OperationStatus { Status = false, Message = "Not found" };
            }

            return ret;
        }       
    }
}
