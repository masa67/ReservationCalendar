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
        public ICollection<CalendarLayer> calendarLayers { get; set; }
        public CalendarLayer combinedCalendar { get; set; }

        public ReservationBookAbs()
        {
        }

        public ReservationBookAbs(ReservationBook rBook, TimePeriod timePeriod, Boolean inclCalLayers, Boolean inclCBAlloc, Boolean inclComb)
        {
            reservationBook = rBook;

            if (inclCalLayers)
            {
                calendarLayers = new List<CalendarLayer>();

                if (timePeriod == null)
                {
                    timePeriod = new TimePeriod { unitsAsDays = true, startTime = rBook.StartTime, endTime = rBook.EndTime };
                }
                foreach (CalendarBookAllocation cal in rBook.CalendarBookAllocations)
                {
                    CalendarLayer calTempl;

                    switch (cal.CalendarDbType)
                    {
                        case CalendarDbType.Absolute:
                            calTempl = new CalendarLayer(cal.AbsCalendarLayer, timePeriod);
                            break;
                        case CalendarDbType.Relative:
                            calTempl = new CalendarLayer(cal.RelCalendarLayer, timePeriod);
                            break;
                        default:
                            throw new ArgumentException("Invalid CalendarType", "rBook");
                    }

                    calTempl.weight = cal.Weight;
                    calendarLayers.Add(calTempl);
                }
            }

            if (!inclCBAlloc)
            {
                reservationBook.CalendarBookAllocations = null;
            }

            if (inclCalLayers && inclComb)
            {
                combinedCalendar = new CalendarLayer(calendarLayers);
            }
        }
    }
}