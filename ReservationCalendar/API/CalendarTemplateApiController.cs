using Newtonsoft.Json;
using ReservationCalendar.DAL;
using ReservationCalendar.Models;
using ReservationCalendar.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace ReservationCalendar.API
{
    public class CalendarTemplateApiController : ApiController
    {
        private ReservationCalendarContext db = new ReservationCalendarContext();

        // POST: api/CalendarTemplateApi/Edit
        [HttpPost]
        [ResponseType(typeof(OperationStatus))]
        public async Task<OperationStatus> Edit(int id, [FromBody] CalendarTemplateEditReq req)
        {
            OperationStatus ret = null;

            if (ModelState.IsValid)
            {
                try
                {
                    foreach (TimeSlot timeSlot in req.calendarTemplate.timeSlots)
                    {
                        AbsTimeSlot absTimeSlot = new AbsTimeSlot(timeSlot);

                        if (absTimeSlot.ID == 0)
                        {
                            db.AbsTimeSlots.Add(absTimeSlot);
                            
                        }
                        else
                        {
                            db.AbsTimeSlots.Attach(absTimeSlot);
                            db.Entry(absTimeSlot).State = EntityState.Modified;
                        }
                    }

                    foreach (TimeSlot timeSlot in req.delTimeSlots)
                    {
                        AbsTimeSlot absTimeSlot = new AbsTimeSlot(timeSlot);

                        db.AbsTimeSlots.Remove(absTimeSlot);
                    }

                    await db.SaveChangesAsync();

                    ret = new OperationStatus { Status = true };
                }
                catch (DataException dex)
                {
                    ret = new OperationStatus { Status = false, Message = "DB save failed" };
                }
            }   
            else
            {
                ret = new OperationStatus { Status = false, Message = "Invalid request" };
            }

            return ret;
        }
    }
}
