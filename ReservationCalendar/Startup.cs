using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ReservationCalendar.Startup))]
namespace ReservationCalendar
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
