﻿using ReservationCalendar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.API
{
    public class CalendarLayerEditReq
    {
        public CalendarLayer calendarLayer { get; set; }
        public long startTime { get; set; }
        public long endTime { get; set; }

        public ICollection<TimeSlot> delTimeSlots { get; set; }

        CalendarLayerEditReq() { }
    }
}