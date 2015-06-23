using ReservationCalendar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.API
{
    public class CalendarLayerEditResp
    {
        public ICollection<TimeSlot> timeSlots { get; set; }

        public long startTime { get; set; }
        public long endTime { get; set; }

        public CalendarLayerEditResp() { }
    }
}