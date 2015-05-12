using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public enum Weekday
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

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
}