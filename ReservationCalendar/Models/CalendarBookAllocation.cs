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
        public CalendarType CalendarType { get; set; }
        public int? AbsCalendarTemplateID { get; set; }
        public int? RelCalendarTemplateID { get; set; }
        public int Weight { get; set; } // 0...100

        public virtual ReservationBook ReservationBook { get; set; }
        public virtual AbsCalendarTemplate AbsCalendarTemplate { get; set; }
        public virtual RelCalendarTemplate RelCalendarTemplate { get; set; }
    }
}