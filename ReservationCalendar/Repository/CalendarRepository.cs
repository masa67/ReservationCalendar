using ReservationCalendar.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Repository
{
    public class CalendarRepository : RepositoryBase<ReservationCalendarContext>
    {
        public CalendarRepository() : base() { }
        public CalendarRepository(string connection) : base(connection) { }
    }
}