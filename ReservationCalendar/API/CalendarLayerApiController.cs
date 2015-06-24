﻿using Newtonsoft.Json;
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
    public class CalendarLayerApiController : ApiController
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

        private async Task<List<AbsTimeSlot>> queryTS(int calDbId, long startTime, long endTime)
        {
            List<AbsTimeSlot> ret = await db.AbsTimeSlots.AsNoTracking().Where(
                t => t.AbsCalendarLayerID == calDbId &&
                    ((t.StartTime >= startTime && t.StartTime < endTime) ||
                     (t.EndTime > startTime && t.EndTime <= endTime) ||
                     (t.StartTime < startTime && t.EndTime > endTime))).ToListAsync();

            return ret;
        }

        // POST: api/CalendarLayerApi/Edit
        [HttpPost]
        [ResponseType(typeof(OperationStatus))]
        public async Task<OperationStatus> Edit(int id, [FromBody] CalendarLayerEditReq req)
        {
            ICollection<AbsTimeSlot> updTimeSlots = new List<AbsTimeSlot>();
            CalendarLayerEditResp resp = null;
            OperationStatus ret = null;

            if (ModelState.IsValid)
            {
                try
                {
                    for (int i = 0; i < req.updTimeSlots.Count; i++)
                    {
                        for (int j = i + 1; j < req.updTimeSlots.Count; j++)
                        {
                            if (req.updTimeSlots.ElementAt(i).checkOverlap(req.updTimeSlots.ElementAt(j)) != TimeSlotOverlap.None)
                            {
                                throw new System.ApplicationException("Overlapping timeslots in request");
                            }
                        }
                    }

                    foreach (TimeSlot timeSlot in req.updTimeSlots)
                    {
                        AbsTimeSlot aTS = new AbsTimeSlot(timeSlot);

                        storedTimeSlots = await queryTS(timeSlot.calDbId, timeSlot.startTime, timeSlot.endTime);

                        if (storedTimeSlots.Count > 0)
                        {
                            foreach (AbsTimeSlot sTS in storedTimeSlots)
                            {
                                Boolean found = false;

                                foreach (TimeSlot ts in req.updTimeSlots)
                                {
                                    if (ts.dbId == sTS.ID)
                                    {
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    foreach (TimeSlot ts in req.delTimeSlots)
                                    {
                                        if (ts.dbId == sTS.ID)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                }

                                if (!found)
                                {
                                    throw new System.ApplicationException("Timeslot in request overlaps with timeslot in DB");
                                }
                            }
                        }                       

                        if (aTS.ID == 0)
                        {

                            db.AbsTimeSlots.Add(aTS);
                        }
                        else
                        {
                            db.AbsTimeSlots.Attach(aTS);
                            db.Entry(aTS).State = EntityState.Modified;
                        }

                        updTimeSlots.Add(aTS);
                    }

                    foreach (TimeSlot timeSlot in req.delTimeSlots)
                    {
                        AbsTimeSlot aTS = new AbsTimeSlot(timeSlot);

                        db.AbsTimeSlots.Attach(aTS);
                        db.AbsTimeSlots.Remove(aTS);
                    }

                    await db.SaveChangesAsync();

                    resp = new CalendarLayerEditResp();

                    foreach (AbsTimeSlot updTimeSlot in updTimeSlots)
                    {
                        if (resp.updTimeSlots == null)
                        {
                            resp.updTimeSlots = new List<UpdTimeSlot>();
                        }
                        resp.updTimeSlots.Add(new UpdTimeSlot() { dbId = updTimeSlot.ID, rowVersion = updTimeSlot.RowVersion });
                    }

                    foreach (TimeSlot timeSlot in req.delTimeSlots)
                    {
                        if (resp.delTimeSlots == null)
                        {
                            resp.delTimeSlots = new List<int>();
                        }
                        resp.delTimeSlots.Add(timeSlot.dbId);
                    }

                    ret = new OperationStatus { Status = true, Data = resp };
                }
                catch (Exception ex)
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