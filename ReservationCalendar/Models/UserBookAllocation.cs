using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class UserBookAllocation
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ReservationBookID { get; set; }
        public Boolean Fixed { get; set; }
        public Int64? StartTime { get; set; }
        public Int64? EndTime { get; set; }

        public virtual User User { get; set; }
        public virtual ReservationBook ReservationBook { get; set; }
    }
}