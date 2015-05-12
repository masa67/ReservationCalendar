using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class ReservationBook
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public virtual ICollection<UserBookAllocation> UserBookAllocations { get; set; }
        public virtual ICollection<CalendarBookAllocation> CalendarBookAllocations { get; set; }
    }
}