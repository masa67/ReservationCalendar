using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class CalendarTemplate
    {
        public CalendarSourceType calendarSourceType { get; set; }
        public CalendarDbType? calendarDbType { get; set; }
        public int? dbCalendarTemplateID { get; set; }
        public string description { get; set; }
        public int? weight { get; set; }
        public ICollection<TimeSlot> timeSlots { get; set; }

        # region DB constructors 

        public CalendarTemplate(AbsCalendarTemplate aCal, TimePeriod timePeriod)
        {
            calendarSourceType = CalendarSourceType.Database;
            calendarDbType = CalendarDbType.Absolute;
            dbCalendarTemplateID = aCal.ID;
            description = aCal.Description;
            timeSlots = new List<TimeSlot>();

            if (timePeriod.unitsAsDays)
            {
                foreach (AbsTimeSlot aSlot in aCal.absTimeSlots)
                {
                    if ((aSlot.StartTime >= timePeriod.startTime && aSlot.StartTime <= timePeriod.endTime) ||
                        (aSlot.EndTime >= timePeriod.startTime && aSlot.EndTime <= timePeriod.endTime))
                    {
                        TimeSlot ts = new TimeSlot(aSlot);
                        ts.parentCalendar = this;
                        timeSlots.Add(ts);
                    }
                }
            }
            else
            {
                throw new System.ArgumentException("Handling the case where unitsAsDays is false is missing", "timePeriod");
            }
        }

        public CalendarTemplate(RelCalendarTemplate rCal, TimePeriod timePeriod)
        {
            calendarSourceType = CalendarSourceType.Database;
            calendarDbType = CalendarDbType.Relative;
            dbCalendarTemplateID = rCal.ID;
            description = rCal.Description;
            timeSlots = new List<TimeSlot>();

            if (timePeriod.unitsAsDays)
            {
                DateTime day = timePeriod.startTime;
                while (day <= timePeriod.endTime)
                {
                    if (day >= rCal.ValidStart && day <= rCal.ValidEnd)
                    {
                        foreach (RelTimeSlot rSlot in rCal.relTimeSlots)
                        {
                            if (rCal.RelCalendarType == RelCalendarType.Daily ||
                                (rCal.RelCalendarType == RelCalendarType.Weekly &&
                                 rSlot.Weekday == day.DayOfWeek))
                            {
                                TimeSlot ts = new TimeSlot(rSlot, day);
                                ts.parentCalendar = this;
                                timeSlots.Add(ts);
                            }

                        }
                    }
                    day = day.AddDays(1);
                }
            }
            else
            {
                throw new System.ArgumentException("Handling the case where unitsAsDays is false is missing", "timePeriod");
            }
        }

        # endregion

        #region Layered constructors

        public CalendarTemplate(ICollection<CalendarTemplate> cals)
        {
            calendarSourceType = CalendarSourceType.Layered;
            timeSlots = new List<TimeSlot>();

            List<CalendarTemplate> orderdCals = cals.OrderBy(x => -x.weight).ToList();

            foreach (CalendarTemplate cal in orderdCals)
            {
                foreach (TimeSlot slot in cal.timeSlots)
                {
                    foreach (TimeSlot mSlot in timeSlots)
                    {
                        
                    }
                }
            }
        }

        #endregion
    }
}