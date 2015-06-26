using ReservationCalendar.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class CalTimeSlot
    {
        public CalendarDbType calDbType { get; set; }
        public int calDbId { get; set; }
        public int dbId { get; set; }
        public CalTimeSlot origTimeSlot { get; set; }
        public long startTime { get; set; }
        public long endTime { get; set; }
        public TimeSlotStatus timeSlotStatus { get; set; }
        public string description { get; set; }
        public int? meetingId { get; set; }
        public Meeting meeting { get; set; }

        public byte[] rowVersion { get; set; }

        public CalTimeSlot() { }

        public CalTimeSlot(AbsTimeSlot aSlot)
        {
            calDbType = CalendarDbType.Absolute;
            calDbId = aSlot.AbsCalendarLayerID;
            dbId = aSlot.ID;
            startTime = aSlot.StartTime;
            endTime = aSlot.EndTime;
            timeSlotStatus = aSlot.TimeSlotStatus;
            description = aSlot.Description;
            meeting = aSlot.Meeting;

            // The following prevents sending AbsTimeSlot data twice on Web API
            if (meeting != null)
            {
                meeting.AbsTimeSlot = null;
            }

            rowVersion = aSlot.RowVersion;
        }

        public CalTimeSlot(RelTimeSlot rSlot, long timeBase)
        {
            calDbType = CalendarDbType.Relative;
            calDbId = rSlot.RelCalendarLayerID;
            dbId = rSlot.ID;

            startTime = TimeHelper.DateTimeToUTCTimeStamp(
                TimeHelper.UTCTimeStampToLocalDateTime(timeBase).AddHours(rSlot.StartTimeHrs).AddMinutes(rSlot.StartTimeMin), false
            );
            endTime = TimeHelper.DateTimeToUTCTimeStamp(
                TimeHelper.UTCTimeStampToLocalDateTime(timeBase).AddHours(rSlot.EndTimeHrs).AddMinutes(rSlot.EndTimeMin), false
            );

            timeSlotStatus = rSlot.TimeSlotStatus;
            description = rSlot.Description;
            rowVersion = rSlot.RowVersion;
        }

        public CalTimeSlot(CalTimeSlot tSlot)
        {
            calDbType = tSlot.calDbType;
            calDbId = tSlot.calDbId;
            dbId = tSlot.dbId;

            origTimeSlot = tSlot;
            startTime = tSlot.startTime;
            endTime = tSlot.endTime;

            timeSlotStatus = tSlot.timeSlotStatus;
            description = tSlot.description;
            meetingId = tSlot.meetingId;
            meeting = tSlot.meeting;

            rowVersion = tSlot.rowVersion;
        }

        public TimeSlotOverlap checkOverlap(CalTimeSlot ts)
        {
            
            if (startTime >= ts.endTime || endTime <= ts.startTime)
            {
                return TimeSlotOverlap.None;
            }
            
            if (startTime > ts.startTime)
            {
                if (endTime >= ts.endTime)
                {
                    return TimeSlotOverlap.LateOverlap;
                }
                else
                {
                    return TimeSlotOverlap.SplitOverlap;
                }
            }
            else
            {
                if (endTime >= ts.endTime)
                {
                    return TimeSlotOverlap.Override;
                }
                else
                {
                    return TimeSlotOverlap.EarlyOverlap;
                }
            }
        }
    }
}