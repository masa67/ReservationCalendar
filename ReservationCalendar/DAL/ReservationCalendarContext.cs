using ReservationCalendar.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ReservationCalendar.DAL
{
    public class ReservationCalendarContext : DbContext
    {
        public ReservationCalendarContext() : base("ReservationCalendarContext")
        {
        }

        public DbSet<AbsCalendarLayer> AbsCalendarLayers { get; set; }
        public DbSet<AbsTimeSlot> AbsTimeSlots { get; set; }
        public DbSet<CalendarBookAllocation> CalendarBookAllocations { get; set; }
        public DbSet<RelCalendarLayer> RelCalendarLayers { get; set; }
        public DbSet<RelTimeSlot> RelTimeSlots { get; set; }
        public DbSet<ReservationBook> ReservationBooks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserBookAllocation> UserBookAllocations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}