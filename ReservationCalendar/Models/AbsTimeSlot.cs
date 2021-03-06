﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace ReservationCalendar.Models
{
    public class AbsTimeSlot
    {
        public int ID { get; set; }
        public int AbsCalendarLayerID { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public TimeSlotStatus TimeSlotStatus { get; set; }
        public string Description { get; set; }
        
        [Timestamp]
        public byte[] RowVersion { get; set; }

        // public virtual AbsCalendarLayer AbsCalendarLayer { get; set; }
        public virtual Meeting Meeting { get; set; }

        public AbsTimeSlot() { }

        public AbsTimeSlot(CalTimeSlot tSlot)
        {
            ID = tSlot.dbId;
            AbsCalendarLayerID = tSlot.calDbId;
            StartTime = tSlot.startTime;
            EndTime = tSlot.endTime;
            TimeSlotStatus = tSlot.timeSlotStatus;
            Description = tSlot.description;
            Meeting = tSlot.meeting;
            RowVersion = tSlot.rowVersion;
        }
    }
}