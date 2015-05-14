using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public enum TimeSlotStatus
    {
        Free,
        Reserved,
        Excluded
    }

    public enum CalendarType
    {
        Absolute,
        Relative
    }

    public class TimePeriod
    {
        public Boolean unitsAsDays { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
    }
}