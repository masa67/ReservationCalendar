using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public enum TimeSlotStatus
    {
        Free = 0,
        Reserved = 1,
        Excluded = 2
    }

    public enum CalendarDbType
    {
        Absolute = 0,
        Relative = 1
    }

    public enum CalendarSourceType
    {
        Database = 0,
        Layered = 1
    }

    public enum TimeSlotOverlap
    {
        None = 0,
        LateOverlap = 1,
        EarlyOverlap = 2,
        Override = 3,
        SplitOverlap = 4
    }

    public class TimePeriod
    {
        public Boolean unitsAsDays { get; set; }
        public long startTime { get; set; }
        public long endTime { get; set; }
    }
}