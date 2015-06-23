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

    public class RelCalendarLayer
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public RelCalendarType RelCalendarType { get; set; }
        public long ValidStart { get; set; }
        public long ValidEnd { get; set; }
        public Boolean UseMerging { get; set; }

        public virtual ICollection<RelTimeSlot> relTimeSlots { get; set; }
    }
}