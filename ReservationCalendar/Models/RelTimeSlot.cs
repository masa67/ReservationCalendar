using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class RelTimeSlot
    {
        public int ID { get; set; }
        public int RelCalendarLayerID { get; set; }
        public DayOfWeek? Weekday { get; set; }
        public Boolean FullDay { get; set; }
        public int StartTimeHrs { get; set; }
        public int StartTimeMin { get; set; }
        public int EndTimeHrs { get; set; }
        public int EndTimeMin { get; set; }
        public TimeSlotStatus TimeSlotStatus { get; set; }
        public string Description { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual RelCalendarLayer RelCalendarLayer { get; set; }
    }
}