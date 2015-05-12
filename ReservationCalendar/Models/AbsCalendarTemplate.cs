using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class AbsCalendarTemplate
    {
        public int ID { get; set; }
        public string Description { get; set; }

        public virtual ICollection<AbsTimeSlot> absTimeSlots { get; set; }
    }
}