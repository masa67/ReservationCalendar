using ReservationCalendar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.API
{
    public class UpdTimeSlot
    {
        public int dbId;
        public byte[] rowVersion;

        public UpdTimeSlot() { }
    }

    public class CalendarLayerEditResp
    {
        public ICollection<UpdTimeSlot> updTimeSlots { get; set; }
        public ICollection<int> delTimeSlots { get; set; }
        
        public CalendarLayerEditResp() { }
    }
}