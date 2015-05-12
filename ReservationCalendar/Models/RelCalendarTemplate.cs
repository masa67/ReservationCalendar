using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public enum RelCalendarType {
        Daily,
        Weekly
    }

    public class RelCalendarTemplate
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public RelCalendarType RelCalendarType { get; set; }
        public DateTime ValidStart { get; set; }
        public DateTime ValidEnd { get; set; }

        public virtual ICollection<RelTimeSlot> relTimeSlots { get; set; }
    }
}