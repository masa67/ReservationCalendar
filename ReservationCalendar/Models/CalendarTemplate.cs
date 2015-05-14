using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class CalendarTemplate
    {
        public CalendarType dbCalendarType { get; set; }
        public int dbCalendarTemplateID { get; set; }
        public string description { get; set; }
        public int weight { get; set; }
        public ICollection<TimeSlot> timeSlots { get; set; }

        public CalendarTemplate(AbsCalendarTemplate aCal, TimePeriod timePeriod)
        {
            dbCalendarType = CalendarType.Absolute;
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
                        timeSlots.Add(new TimeSlot(aSlot));
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
            dbCalendarType = CalendarType.Relative;
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
                                timeSlots.Add(new TimeSlot(rSlot, day));
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
    }
}