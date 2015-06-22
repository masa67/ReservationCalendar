using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReservationCalendar.Helpers;
using ReservationCalendar.Models;

namespace ReservationCalendar.DAL
{
    public class ReservationCalendarInitializer : System.Data.Entity.DropCreateDatabaseAlways<ReservationCalendarContext>
    {
        protected override void Seed(ReservationCalendarContext context)
        {
            var users = new List<User>
            {
                new User{LastName="Doe", FirstName="John"},
                new User{LastName="Doe", FirstName="Jane"},
            };

            users.ForEach(u => context.Users.Add(u));
            context.SaveChanges();

            var reservationBooks = new List<ReservationBook>
            {
                new ReservationBook{Description="Book #1",
                                    StartTime=TimeHelper.DateTimeToUTCTimeStamp(new DateTime(2015, 1, 1), false),
                                    EndTime=TimeHelper.DateTimeToUTCTimeStamp(new DateTime(2015, 12, 31), false)},
                new ReservationBook{Description="Book #2",
                                    StartTime=TimeHelper.DateTimeToUTCTimeStamp(new DateTime(2015, 1, 1), false),
                                    EndTime=TimeHelper.DateTimeToUTCTimeStamp(new DateTime(2015, 12, 31), false)}
            };

            reservationBooks.ForEach(r => context.ReservationBooks.Add(r));
            context.SaveChanges();

            var userBookAllocations = new List<UserBookAllocation>
            {
                new UserBookAllocation{UserID=1, ReservationBookID=1, Fixed=true},
                new UserBookAllocation{UserID=2, ReservationBookID=2, Fixed=true}
            };

            userBookAllocations.ForEach(a => context.UserBookAllocations.Add(a));
            context.SaveChanges();

            var relCalendarTemplates = new List<RelCalendarTemplate>
            {
                new RelCalendarTemplate{Description="standard weekly",
                                        RelCalendarType=RelCalendarType.Weekly,
                                        ValidStart=TimeHelper.DateTimeToUTCTimeStamp(new DateTime(2015, 5, 1), false),
                                        ValidEnd=TimeHelper.DateTimeToUTCTimeStamp(new DateTime(2015, 5, 31), false),
                                        UseMerging=false}
            };

            relCalendarTemplates.ForEach(c => context.RelCalendarTemplates.Add(c));
            context.SaveChanges();

            var relTimeSlots = new List<RelTimeSlot>
            {
                new RelTimeSlot{RelCalendarTemplateID=1, Weekday=DayOfWeek.Monday, StartTimeHrs=8, StartTimeMin=0, EndTimeHrs=16, EndTimeMin=0, TimeSlotStatus=TimeSlotStatus.Free},
                new RelTimeSlot{RelCalendarTemplateID=1, Weekday=DayOfWeek.Tuesday, StartTimeHrs=8, StartTimeMin=0, EndTimeHrs=16, EndTimeMin=0, TimeSlotStatus=TimeSlotStatus.Free},
                new RelTimeSlot{RelCalendarTemplateID=1, Weekday=DayOfWeek.Wednesday, StartTimeHrs=8, StartTimeMin=0, EndTimeHrs=16, EndTimeMin=0, TimeSlotStatus=TimeSlotStatus.Free},
                new RelTimeSlot{RelCalendarTemplateID=1, Weekday=DayOfWeek.Thursday, StartTimeHrs=8, StartTimeMin=0, EndTimeHrs=16, EndTimeMin=0, TimeSlotStatus=TimeSlotStatus.Free},
                new RelTimeSlot{RelCalendarTemplateID=1, Weekday=DayOfWeek.Friday, StartTimeHrs=8, StartTimeMin=0, EndTimeHrs=16, EndTimeMin=0, TimeSlotStatus=TimeSlotStatus.Free}
            };

            relTimeSlots.ForEach(s => context.RelTimeSlots.Add(s));
            context.SaveChanges();

            var absCalendarTemplates = new List<AbsCalendarTemplate>
            {
                new AbsCalendarTemplate{Description="public holidays", UseMerging=false},
                new AbsCalendarTemplate{Description="meetings", UseMerging=false},
                new AbsCalendarTemplate{Description="free slots", UseMerging=true}
            };

            absCalendarTemplates.ForEach(c => context.AbsCalendarTemplates.Add(c));
            context.SaveChanges();

            var absTimeSlots = new List<AbsTimeSlot>
            {
                new AbsTimeSlot{
                    AbsCalendarTemplateID=1,
                    StartTime=TimeHelper.DateTimeToUTCTimeStamp(new DateTime(2015, 5, 1), false),
                    EndTime=TimeHelper.DateTimeToUTCTimeStamp(new DateTime(2015, 5, 2), false),
                    TimeSlotStatus=TimeSlotStatus.Excluded,
                    Description="1st of May"},
                new AbsTimeSlot{
                    AbsCalendarTemplateID=1,
                    StartTime=TimeHelper.DateTimeToUTCTimeStamp(new DateTime(2015, 5, 14), false),
                    EndTime=TimeHelper.DateTimeToUTCTimeStamp(new DateTime(2015, 5, 15), false),
                    TimeSlotStatus=TimeSlotStatus.Excluded,
                    Description="Ascension Day"}
            };

            absTimeSlots.ForEach(s => context.AbsTimeSlots.Add(s));
            context.SaveChanges();

            var calendarBookAllocations = new List<CalendarBookAllocation>
            {
                new CalendarBookAllocation{ReservationBookID=1, CalendarDbType=CalendarDbType.Absolute, AbsCalendarTemplateID=1, Weight=0},
                new CalendarBookAllocation{ReservationBookID=1, CalendarDbType=CalendarDbType.Absolute, AbsCalendarTemplateID=2, Weight=5},
                new CalendarBookAllocation{ReservationBookID=1, CalendarDbType=CalendarDbType.Absolute, AbsCalendarTemplateID=3, Weight=10},
                // new CalendarBookAllocation{ReservationBookID=1, CalendarDbType=CalendarDbType.Relative, RelCalendarTemplateID=1, Weight=15}
            };

            calendarBookAllocations.ForEach(a => context.CalendarBookAllocations.Add(a));
            context.SaveChanges();
        }
    }
}