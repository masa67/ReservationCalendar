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

    public enum CalendarDbType
    {
        Absolute,
        Relative
    }

    public enum CalendarSourceType
    {
        Database,
        Layered
    }

    public enum TimeSlotOverlap
    {
        None,
        LateOverlap,
        EarlyOverlap,
        Override,
        SplitOverlap
    }

    public class TimePeriod
    {
        public Boolean unitsAsDays { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
    }
}