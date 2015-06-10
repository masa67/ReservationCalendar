using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace ReservationCalendar.Models
{
    public class AbsTimeSlot
    {
        public int ID { get; set; }
        public int AbsCalendarTemplateID { get; set; }
        public Boolean FullDay { get; set; }
        public long StartTime { get; set; }
        public long? EndTime { get; set; }
        public TimeSlotStatus TimeSlotStatus { get; set; }
        public string Description { get; set; }

        public virtual AbsCalendarTemplate AbsCalendarTemplate { get; set; }
    }
}