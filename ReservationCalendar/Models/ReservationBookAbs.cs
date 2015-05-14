using ReservationCalendar.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReservationCalendar.Models
{
    public class ReservationBookAbs
    {
        public ReservationBook reservationBook { get; set; }
        public ICollection<CalendarTemplate> calendarTemplates { get; set; }

        public ReservationBookAbs()
        {
        }

        public ReservationBookAbs(ReservationBook rBook)
        {
            reservationBook = rBook;
            calendarTemplates = new List<CalendarTemplate>();

            TimePeriod timePeriod = new TimePeriod{ unitsAsDays=true, startTime=rBook.StartTime, endTime=rBook.EndTime };

            foreach (CalendarBookAllocation cal in reservationBook.CalendarBookAllocations)
            {
                CalendarTemplate calTempl;

                switch (cal.CalendarType)
                {
                    case CalendarType.Absolute:
                        calTempl = new CalendarTemplate(cal.AbsCalendarTemplate, timePeriod);
                        break;
                    case CalendarType.Relative:
                        calTempl = new CalendarTemplate(cal.RelCalendarTemplate, timePeriod);
                        break;
                    default:
                        throw new ArgumentException("Invalid CalendarType", "rBook");
                }

                calTempl.weight = cal.Weight;
                calendarTemplates.Add(calTempl);
            }
        }
    }
}