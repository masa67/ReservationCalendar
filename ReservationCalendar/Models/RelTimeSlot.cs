using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class RelTimeSlot
    {
        public int ID { get; set; }
        public int RelCalendarTemplateID { get; set; }
        public Weekday? Weekday { get; set; }
        public Boolean FullDay { get; set; }
        public int StartTimeHrs { get; set; }
        public int StartTimeMin { get; set; }
        public int EndTimeHrs { get; set; }
        public int EndTimeMin { get; set; }
        public TimeSlotStatus TimeSlotStatus { get; set; }
        public string Description { get; set; }

        public virtual RelCalendarTemplate RelCalendarTemplate { get; set; }
    }
}