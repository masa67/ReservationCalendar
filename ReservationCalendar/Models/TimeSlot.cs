using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class TimeSlot
    {
        public CalendarDbType dbType { get; set; }
        public int dbId { get; set; }
        public TimeSlot origTimeSlot { get; set; }
        public Boolean fullDay { get; set; }
        public DateTime startTime { get; set; }
        public DateTime? endTime { get; set; }
        public TimeSlotStatus timeSlotStatus { get; set; }
        public string description { get; set; }

        public TimeSlot(AbsTimeSlot aSlot)
        {
            dbType = CalendarDbType.Absolute;
            dbId = aSlot.ID;
            fullDay = aSlot.FullDay;
            startTime = aSlot.StartTime;

            if (fullDay)
            {
                endTime = aSlot.EndTime;
            }
            timeSlotStatus = aSlot.TimeSlotStatus;
            description = aSlot.Description;
        }

        public TimeSlot(RelTimeSlot rSlot, DateTime timeBase)
        {
            dbType = CalendarDbType.Relative;
            dbId = rSlot.ID;
            fullDay = rSlot.FullDay;

            if (fullDay)
            {
                startTime = timeBase;
                // endTime not used for full-day slots
            }
            else
            {
                startTime = timeBase.AddHours(rSlot.StartTimeHrs).AddMinutes(rSlot.StartTimeMin);
                endTime = timeBase.AddHours(rSlot.EndTimeHrs).AddMinutes(rSlot.EndTimeMin);
            }

            timeSlotStatus = rSlot.TimeSlotStatus;
            description = rSlot.Description;
        }

        public TimeSlot(TimeSlot tSlot)
        {
            dbType = tSlot.dbType;
            dbId = tSlot.dbId;

            origTimeSlot = tSlot;
            fullDay = tSlot.fullDay;
            startTime = tSlot.startTime;

            if (tSlot.endTime != null)
            {
                endTime = tSlot.endTime;
            }

            timeSlotStatus = tSlot.timeSlotStatus;
            description = tSlot.description;
        }

        public TimeSlotOverlap checkOverlap(TimeSlot ts)
        {
            if (fullDay || ts.fullDay)
            {
                DateTime aStartDate = startTime.Date, bStartDate = ts.startTime.Date;

                if (fullDay)
                {
                    if (ts.fullDay)
                    {
                        if (aStartDate.Equals(bStartDate))
                        {
                            return TimeSlotOverlap.Override;
                        }
                        else
                        {
                            return TimeSlotOverlap.None;
                        }
                    }

                    if (aStartDate < bStartDate)
                    {
                        return TimeSlotOverlap.None;
                    }
                    else
                    {
                        DateTime bEndDate = ts.startTime.Date;

                        if (aStartDate.Equals(bStartDate))
                        {
                            if (aStartDate.Equals(bEndDate))
                            {
                                return TimeSlotOverlap.Override;
                            }
                            else
                            {
                                return TimeSlotOverlap.EarlyOverlap;
                            }
                        }
                        else
                        {
                            if (aStartDate > bEndDate)
                            {
                                return TimeSlotOverlap.None;
                            }
                            else
                            {
                                if (aStartDate.Equals(bEndDate))
                                {
                                    return TimeSlotOverlap.LateOverlap;
                                } 
                                else
                                {
                                    return TimeSlotOverlap.SplitOverlap;
                                }

                            }
                        }
                    }
                }
            }
            
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