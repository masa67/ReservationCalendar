using Newtonsoft.Json;
using ReservationCalendar.DAL;
using ReservationCalendar.Interfaces;
using ReservationCalendar.Models;
using ReservationCalendar.Repository;
using ReservationCalendar.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace ReservationCalendar.API
{
    public class ConcurrencyConflictException : System.Exception
    {
        public ConcurrencyConflictException() : base() { }
        public ConcurrencyConflictException(string message) : base(message) { }
        public ConcurrencyConflictException(string message, System.Exception inner) : base(message, inner) { }
    }

    public class CalendarLayerApiController : ApiController
    {
        private static Object _lock = new Object();
        private IGenericRepository _calendarRepository;
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

            throw new ConcurrencyConflictException();
        }

        private List<AbsTimeSlot> queryTS(int calDbId, long startTime, long endTime)
        {
            List<AbsTimeSlot> ret = _calendarRepository.QueryNoTracking<AbsTimeSlot>(
                t => t.AbsCalendarLayerID == calDbId &&
                    ((t.StartTime >= startTime && t.StartTime < endTime) ||
                     (t.EndTime > startTime && t.EndTime <= endTime) ||
                     (t.StartTime < startTime && t.EndTime > endTime))).ToList();

            return ret;
        }

        public CalendarLayerApiController()
        {
            _calendarRepository = new CalendarRepository();
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

                    // Only one HTTP request at a time is allowed to be under process in the following code block.
                    // Otherwise duplicated requests (e.g., due to duplicated form submits) would not be checked correctly against duplication.
                    lock (_lock)
                    {

                        foreach (CalTimeSlot timeSlot in req.updTimeSlots)
                        {
                            AbsTimeSlot aTS = new AbsTimeSlot(timeSlot);

                            storedTimeSlots = queryTS(timeSlot.calDbId, timeSlot.startTime, timeSlot.endTime);

                            if (storedTimeSlots.Count > 0)
                            {
                                foreach (AbsTimeSlot sTS in storedTimeSlots)
                                {
                                    Boolean found = false;

                                    foreach (CalTimeSlot ts in req.updTimeSlots)
                                    {
                                        if (ts.dbId == sTS.ID)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }

                                    if (!found)
                                    {
                                        foreach (CalTimeSlot ts in req.delTimeSlots)
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
                                        throw new ConcurrencyConflictException();
                                    }
                                }
                            }

                            if (aTS.ID == 0)
                            {
                                _calendarRepository.Add<AbsTimeSlot>(aTS);
                                if (aTS.Meeting != null)
                                {
                                    _calendarRepository.Add<Meeting>(aTS.Meeting);
                                }
                            }
                            else
                            {
                                if (aTS.Meeting != null)
                                {
                                    aTS.Meeting.MeetingId = aTS.ID;
                                }

                                _calendarRepository.UpdateNoSave<AbsTimeSlot>(aTS);

                                if (aTS.Meeting != null)
                                {
                                    if (aTS.Meeting.RowVersion == null)
                                    {
                                        _calendarRepository.Add<Meeting>(aTS.Meeting);
                                    }
                                    else
                                    {
                                        _calendarRepository.UpdateNoSave<Meeting>(aTS.Meeting);
                                    }
                                }
                            }

                            updTimeSlots.Add(aTS);
                        }

                        foreach (CalTimeSlot timeSlot in req.delTimeSlots)
                        {
                            AbsTimeSlot aTS = new AbsTimeSlot(timeSlot);

                            if (aTS.Meeting != null)
                            {
                                _calendarRepository.DeleteNoSave<Meeting>(aTS.Meeting);
                            }

                            _calendarRepository.DeleteNoSave<AbsTimeSlot>(aTS);
                        }

                        _calendarRepository.SaveChanges();
                    }

                    resp = new CalendarLayerEditResp();

                    foreach (AbsTimeSlot updTimeSlot in updTimeSlots)
                    {
                        UpdTimeSlot uts;

                        if (resp.updTimeSlots == null)
                        {
                            resp.updTimeSlots = new List<UpdTimeSlot>();
                        }

                        uts = new UpdTimeSlot() { dbId = updTimeSlot.ID, rowVersion = updTimeSlot.RowVersion };
                        if (updTimeSlot.Meeting != null)
                        {
                            uts.rowVersionMeeting = updTimeSlot.Meeting.RowVersion;
                        }
                        resp.updTimeSlots.Add(uts);
                    }

                    foreach (CalTimeSlot timeSlot in req.delTimeSlots)
                    {
                        if (resp.delTimeSlots == null)
                        {
                            resp.delTimeSlots = new List<int>();
                        }
                        resp.delTimeSlots.Add(timeSlot.dbId);
                    }

                    ret = new OperationStatus { Status = true, Data = resp };
                }
                catch (ConcurrencyConflictException ex)
                {
                    ret = OperationStatus.CreateFromException("Concurrency conflict", ex);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ret = OperationStatus.CreateFromException("Concurrency conflict", ex);
                }
                catch (Exception ex)
                {
                    ret = OperationStatus.CreateFromException("DB save failed", ex);
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
