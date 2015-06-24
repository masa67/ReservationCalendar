using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace ReservationCalendar.Models
{
    public class AbsCalendarLayer
    {
        public int ID { get; set; }
        public CalendarLayerType Type { get; set; }
        public string Description { get; set; }
        public Boolean UseMerging { get; set; }

        public virtual ICollection<AbsTimeSlot> absTimeSlots { get; set; }
    }
}