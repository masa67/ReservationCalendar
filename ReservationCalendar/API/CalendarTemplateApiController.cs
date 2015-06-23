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
        private List<AbsTimeSlot> storedTimeSlots;

        private void deleteTSFromStoredList(AbsTimeSlot ts)
        {
            foreach (AbsTimeSlot bTS in storedTimeSlots)
            {
                if (ts.ID == bTS.ID)
                {
                    storedTimeSlots.Remove(bTS);
                    return;
                }
            }

            throw new System.ApplicationException("DB concurrency conflict detected");
        }

        private async Task<List<AbsTimeSlot>> queryTS(CalendarTemplateEditReq req)
        {
            List<AbsTimeSlot> ret = await db.AbsTimeSlots.AsNoTracking().Where(
                t => t.AbsCalendarTemplateID == req.calendarTemplate.dbCalendarTemplateID &&
                    ((t.StartTime >= req.startTime && t.StartTime < req.endTime) ||
                     (t.EndTime > req.startTime && t.EndTime <= req.endTime))).ToListAsync();

            return ret;
        }

        // POST: api/CalendarTemplateApi/Edit
        [HttpPost]
        [ResponseType(typeof(OperationStatus))]
        public async Task<OperationStatus> Edit(int id, [FromBody] CalendarTemplateEditReq req)
        {
            ICollection<TimeSlot> timeSlots = req.calendarTemplate.timeSlots;
            OperationStatus ret = null;

            if (ModelState.IsValid)
            {
                try
                {
                    storedTimeSlots = await queryTS(req);

                    for (int i = 0; i < timeSlots.Count; i++)
                    {
                        for (int j = i + 1; j < timeSlots.Count; j++)
                        {
                            if (timeSlots.ElementAt(i).checkOverlap(timeSlots.ElementAt(j)) != TimeSlotOverlap.None) {
                                throw new System.ApplicationException("Overlapping events in request");
                            }
                        }
                    }

                    foreach (TimeSlot timeSlot in req.calendarTemplate.timeSlots)
                    {
                        AbsTimeSlot aTS = new AbsTimeSlot(timeSlot);

                        if (aTS.ID == 0)
                        {
                            db.AbsTimeSlots.Add(aTS);
                            
                        }
                        else
                        {
                            deleteTSFromStoredList(aTS);
                            db.AbsTimeSlots.Attach(aTS);
                            db.Entry(aTS).State = EntityState.Modified;
                        }
                    }

                    foreach (TimeSlot timeSlot in req.delTimeSlots)
                    {
                        AbsTimeSlot aTS = new AbsTimeSlot(timeSlot);

                        deleteTSFromStoredList(aTS);
                        db.AbsTimeSlots.Attach(aTS);
                        db.AbsTimeSlots.Remove(aTS);
                    }

                    if (storedTimeSlots.Count != 0)
                    {
                        throw new System.ApplicationException("DB concurrency conflict detected");
                    }

                    await db.SaveChangesAsync();

                    storedTimeSlots = await queryTS(req);
                    timeSlots = new List<TimeSlot>();
                    foreach (AbsTimeSlot aTS in storedTimeSlots)
                    {
                        timeSlots.Add(new TimeSlot(aTS));
                    }

                    ret = new OperationStatus { Status = true, Data = timeSlots };
                }
                catch (Exception ex)
                {
                    ret = new OperationStatus { Status = false, Message = "DB save failed" };
                }

                try
                {
                    storedTimeSlots = await queryTS(req);
                    timeSlots = new List<TimeSlot>();
                    foreach (AbsTimeSlot aTS in storedTimeSlots)
                    {
                        timeSlots.Add(new TimeSlot(aTS));
                    }

                    ret.Data = timeSlots;
                }
                catch (Exception ex)
                {
                    ret.Status = false;
                    ret.Message = "DB read failure";
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
