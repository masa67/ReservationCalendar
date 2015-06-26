using ReservationCalendar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.API
{
    public class CalendarLayerEditReq
    {
        public ICollection<CalTimeSlot> delTimeSlots { get; set; }
        public ICollection<CalTimeSlot> updTimeSlots { get; set; }

        CalendarLayerEditReq() { }
    }
}