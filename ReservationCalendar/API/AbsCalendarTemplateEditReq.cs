using ReservationCalendar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.API
{
    public class AbsCalendarTemplateEditReq
    {
        int id { get; set; }
        AbsCalendarTemplate absCalendarTemplate { get; set; }
        long startTime { get; set; }
        long endTime { get; set; }
    }
}