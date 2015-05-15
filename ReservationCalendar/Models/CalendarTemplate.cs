using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class TimeSlotConflict
    {
        public TimeSlot aSlot { get; set; }
        public TimeSlot bSlot { get; set; }
        public TimeSlotOverlap timeSlotOverlap { get; set; }
    }

    public class CalendarTemplate
    {
        public CalendarSourceType calendarSourceType { get; set; }
        public CalendarDbType? calendarDbType { get; set; }
        public int? dbCalendarTemplateID { get; set; }
        public string description { get; set; }
        public int? weight { get; set; }
        public ICollection<TimeSlot> timeSlots { get; set; }
        public ICollection<TimeSlotConflict> timeSlotConflicts { get; set; }

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
            timeSlotConflicts = new List<TimeSlotConflict>();

            List<CalendarTemplate> orderdCals = cals.OrderBy(x => -x.weight).ToList();

            foreach (CalendarTemplate cal in orderdCals)
            {
                foreach (TimeSlot slot in cal.timeSlots)
                {
                    ICollection<TimeSlot> timeSlotsToDelete = new List<TimeSlot>();

                    foreach (TimeSlot mSlot in timeSlots)
                    {
                        TimeSlotOverlap tsCmp = slot.checkOverlap(mSlot);

                        if (tsCmp != TimeSlotOverlap.None)
                        {
                            timeSlotConflicts.Add(new TimeSlotConflict { aSlot = slot, bSlot = mSlot, timeSlotOverlap = tsCmp });
                        }

                        switch (slot.checkOverlap(mSlot))
                        {
                            case TimeSlotOverlap.None:
                                break;
                            case TimeSlotOverlap.LateOverlap:
                                mSlot.fullDay = false;
                                mSlot.endTime = slot.startTime;
                                break;
                            case TimeSlotOverlap.EarlyOverlap:
                                if (mSlot.fullDay) {
                                    mSlot.fullDay = false;
                                    mSlot.endTime = mSlot.startTime.AddDays(1);
                                }
                                if (slot.fullDay)
                                {
                                    mSlot.startTime = slot.startTime.AddDays(1);
                                }
                                else
                                {
                                    mSlot.startTime = slot.endTime ?? default(DateTime);
                                }
                                break;
                            case TimeSlotOverlap.Override:
                                timeSlotsToDelete.Add(mSlot);
                                break;
                            case TimeSlotOverlap.SplitOverlap:
                                TimeSlot dSlot = new TimeSlot(mSlot);
                                dSlot.origTimeSlot = mSlot.origTimeSlot;

                                mSlot.endTime = slot.startTime;
                                if (slot.fullDay)
                                {
                                    dSlot.startTime = slot.startTime.AddDays(1);
                                }
                                else
                                {
                                    dSlot.startTime = slot.endTime ?? default(DateTime);
                                }
                                timeSlots.Add(dSlot);
                                break;
                            default:
                                throw new InvalidOperationException("Unhandled return value of TimeSlot.checkOverlap()");
                        }
                    }

                    foreach (TimeSlot dSlot in timeSlotsToDelete)
                    {
                        timeSlots.Remove(dSlot);
                    }

                    timeSlots.Add(new TimeSlot(slot));
                }
            }

            timeSlots = timeSlots.OrderBy(x => x.startTime).ToList();
        }

        #endregion
    }
}