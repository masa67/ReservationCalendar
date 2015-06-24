using ReservationCalendar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.API
{
    public class CalendarLayerEditReq
    {
        public ICollection<TimeSlot> delTimeSlots { get; set; }
        public ICollection<TimeSlot> updTimeSlots { get; set; }

        CalendarLayerEditReq() { }
    }
}