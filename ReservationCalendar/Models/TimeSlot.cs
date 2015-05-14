﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class TimeSlot
    {
        public int dbId { get; set; }
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
    }
}