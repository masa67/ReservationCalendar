using ReservationCalendar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.API
{
    public class AbsCalendarTemplateEditReq
    {
        public int id { get; set; }
        public AbsCalendarTemplate absCalendarTemplate { get; set; }
        public long startTime { get; set; }
        public long endTime { get; set; }
    }
}