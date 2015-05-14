using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class TimeSlot
    {
        public int? dbId { get; set; }
        public CalendarTemplate parentCalendar { get; set; } // in combined calendars, we need to know where the time slots are coming from
        public Boolean fullDay { get; set; }
        public DateTime startTime { get; set; }
        public DateTime? endTime { get; set; }
        public TimeSlotStatus timeSlotStatus { get; set; }
        public string description { get; set; }

        public TimeSlot(AbsTimeSlot aSlot)
        {
            dbId = aSlot.ID;
            fullDay = aSlot.FullDay;
            startTime = aSlot.StartTime;

            if (fullDay)
            {
                endTime = aSlot.EndTime;
            }
            timeSlotStatus = aSlot.TimeSlotStatus;
            description = aSlot.Description;
        }

        public TimeSlot(RelTimeSlot rSlot, DateTime timeBase)
        {
            dbId = rSlot.ID;
            fullDay = rSlot.FullDay;

            if (fullDay)
            {
                startTime = timeBase;
                // endTime not used for full-day slots
            }
            else
            {
                startTime = timeBase.AddHours(rSlot.StartTimeHrs).AddMinutes(rSlot.StartTimeMin);
                endTime = timeBase.AddHours(rSlot.EndTimeHrs).AddMinutes(rSlot.EndTimeMin);
            }

            timeSlotStatus = rSlot.TimeSlotStatus;
            description = rSlot.Description;
        }

        public TimeSlotOverlap checkOverlap(TimeSlot ts)
        {
            if (startTime >= ts.endTime || endTime <= ts.startTime)
            {
                return TimeSlotOverlap.None;
            }
            
            if (startTime > ts.startTime)
            {
                if (endTime >= ts.endTime)
                {
                    return TimeSlotOverlap.LateOverlap;
                }
                else
                {
                    return TimeSlotOverlap.SplitOverlap;
                }
            }
            else
            {
                if (endTime >= ts.endTime)
                {
                    return TimeSlotOverlap.Override;
                }
                else
                {
                    return TimeSlotOverlap.EarlyOverlap;
                }
            }
        }
    }
}