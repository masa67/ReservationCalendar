﻿using ReservationCalendar.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Models
{
    public class TimeSlotConflict
    {
        public CalTimeSlot aSlot { get; set; }
        public CalTimeSlot bSlot { get; set; }
        public TimeSlotOverlap timeSlotOverlap { get; set; }
    }

    public class CalendarLayer
    {
        public CalendarSourceType sourceType { get; set; }
        public CalendarDbType? dbType { get; set; }
        public CalendarLayerType? layerType { get; set; }
        public int? dbId { get; set; }
        public string description { get; set; }
        public int? weight { get; set; }
        public Boolean useMerging { get; set; }
        public ICollection<CalTimeSlot> timeSlots { get; set; }
        public ICollection<TimeSlotConflict> timeSlotConflicts { get; set; }

        # region DB constructors 

        public CalendarLayer() { }

        public CalendarLayer(AbsCalendarLayer aCal, TimePeriod timePeriod)
        {
            sourceType = CalendarSourceType.Database;
            dbType = CalendarDbType.Absolute;
            layerType = aCal.Type;
            dbId = aCal.ID;
            description = aCal.Description;
            useMerging = aCal.UseMerging;
            timeSlots = new List<CalTimeSlot>();

            if (timePeriod.unitsAsDays)
            {
                foreach (AbsTimeSlot aSlot in aCal.absTimeSlots)
                {
                    if ((aSlot.StartTime >= timePeriod.startTime && aSlot.StartTime <= timePeriod.endTime) ||
                        (aSlot.EndTime >= timePeriod.startTime && aSlot.EndTime <= timePeriod.endTime))
                    {
                        CalTimeSlot ts = new CalTimeSlot(aSlot);
                        timeSlots.Add(ts);
                    }
                }
            }
            else
            {
                throw new System.ArgumentException("Handling the case where unitsAsDays is false is missing", "timePeriod");
            }
        }

        public CalendarLayer(RelCalendarLayer rCal, TimePeriod timePeriod)
        {
            sourceType = CalendarSourceType.Database;
            dbType = CalendarDbType.Relative;
            dbId = rCal.ID;
            description = rCal.Description;
            useMerging = rCal.UseMerging;
            timeSlots = new List<CalTimeSlot>();

            if (timePeriod.unitsAsDays)
            {
                long day = timePeriod.startTime;
                DateTime dt = TimeHelper.UTCTimeStampToLocalDateTime(timePeriod.startTime);

                while (day <= timePeriod.endTime)
                {
                    if (day >= rCal.ValidStart && day <= rCal.ValidEnd)
                    {
                        foreach (RelTimeSlot rSlot in rCal.relTimeSlots)
                        {
                            if (rCal.RelCalendarType == RelCalendarType.Daily ||
                                (rCal.RelCalendarType == RelCalendarType.Weekly &&
                                 rSlot.Weekday == dt.DayOfWeek))
                            {
                                CalTimeSlot ts = new CalTimeSlot(rSlot, day);
                                timeSlots.Add(ts);
                            }

                        }
                    }
                    dt = dt.AddDays(1);
                    day = TimeHelper.DateTimeToUTCTimeStamp(dt, false);
                }
            }
            else
            {
                throw new System.ArgumentException("Handling the case where unitsAsDays is false is missing", "timePeriod");
            }
        }

        # endregion

        #region Layered constructors

        public CalendarLayer(ICollection<CalendarLayer> cals)
        {
            sourceType = CalendarSourceType.Layered;
            timeSlots = new List<CalTimeSlot>();
            timeSlotConflicts = new List<TimeSlotConflict>();

            List<CalendarLayer> orderdCals = cals.OrderBy(x => -x.weight).ToList();

            foreach (CalendarLayer cal in orderdCals)
            {
                foreach (CalTimeSlot slot in cal.timeSlots)
                {
                    ICollection<CalTimeSlot> timeSlotsToDelete = new List<CalTimeSlot>();

                    foreach (CalTimeSlot mSlot in timeSlots)
                    {
                        TimeSlotOverlap tsCmp = slot.checkOverlap(mSlot);

                        if (tsCmp != TimeSlotOverlap.None)
                        {
                            timeSlotConflicts.Add(new TimeSlotConflict { aSlot = slot, bSlot = mSlot, timeSlotOverlap = tsCmp });
                        }

                        switch (tsCmp)
                        {
                            case TimeSlotOverlap.None:
                                break;
                            case TimeSlotOverlap.LateOverlap:
                                mSlot.endTime = slot.startTime;
                                break;
                            case TimeSlotOverlap.EarlyOverlap:
                                mSlot.endTime = TimeHelper.DateTimeToUTCTimeStamp(TimeHelper.UTCTimeStampToLocalDateTime(mSlot.startTime).AddDays(1), false);
                                mSlot.startTime = slot.endTime;
                                break;
                            case TimeSlotOverlap.Override:
                                timeSlotsToDelete.Add(mSlot);
                                break;
                            case TimeSlotOverlap.SplitOverlap:
                                CalTimeSlot dSlot = new CalTimeSlot(mSlot);
                                dSlot.origTimeSlot = mSlot.origTimeSlot;

                                mSlot.endTime = slot.startTime;
                                dSlot.startTime = TimeHelper.DateTimeToUTCTimeStamp(TimeHelper.UTCTimeStampToLocalDateTime(slot.endTime), false);
                                timeSlots.Add(dSlot);
                                break;
                            default:
                                throw new InvalidOperationException("Unhandled return value of TimeSlot.checkOverlap()");
                        }
                    }

                    foreach (CalTimeSlot dSlot in timeSlotsToDelete)
                    {
                        timeSlots.Remove(dSlot);
                    }

                    timeSlots.Add(new CalTimeSlot(slot));
                }
            }

            timeSlots = timeSlots.OrderBy(x => x.startTime).ToList();
        }

        #endregion
    }
}