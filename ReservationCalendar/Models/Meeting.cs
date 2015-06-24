using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class Meeting
    {
        [Key, ForeignKey("AbsTimeSlot")]
        public int ID { get; set; }       
        public int AbsTimeSlotID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public virtual AbsTimeSlot AbsTimeSlot { get; set; }

        public Meeting() { }
    }
}