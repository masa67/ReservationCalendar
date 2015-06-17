using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace ReservationCalendar.Models
{
    public class AbsTimeSlot
    {
        public int ID { get; set; }
        public int AbsCalendarTemplateID { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public TimeSlotStatus TimeSlotStatus { get; set; }
        public string Description { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual AbsCalendarTemplate AbsCalendarTemplate { get; set; }

        public AbsTimeSlot() { }

        public AbsTimeSlot(TimeSlot tSlot)
        {
            ID = tSlot.dbId;
            AbsCalendarTemplateID = tSlot.calDbId;
            StartTime = tSlot.startTime;
            EndTime = tSlot.endTime;
            TimeSlotStatus = tSlot.timeSlotStatus;
            Description = tSlot.description;
            RowVersion = tSlot.rowVersion;
        }
    }
}