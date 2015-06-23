using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class CalendarBookAllocation
    {
        public int ID { get; set; }
        public int ReservationBookID { get; set; }
        public CalendarDbType CalendarDbType { get; set; }
        public int? AbsCalendarLayerID { get; set; }
        public int? RelCalendarLayerID { get; set; }
        public int Weight { get; set; } // 0...100

        public virtual ReservationBook ReservationBook { get; set; }
        public virtual AbsCalendarLayer AbsCalendarLayer { get; set; }
        public virtual RelCalendarLayer RelCalendarLayer { get; set; }
    }
}