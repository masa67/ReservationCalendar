using Newtonsoft.Json;
using ReservationCalendar.DAL;
using ReservationCalendar.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;

namespace ReservationCalendar.API
{
    public class AbsCalendarTemplateApiController : ApiController
    {
        private ReservationCalendarContext db = new ReservationCalendarContext();

        // POST: api/AbsCalendarTemplateApi/Edit
        [ResponseType(typeof(OperationStatus))]
        public OperationStatus Edit(int id /* AbsCalendarTemplateEditReq absCalendarTemplate */)
        {
            OperationStatus ret = null;

            if (ModelState.IsValid)
            {

                ret = new OperationStatus { Status = true };
            }
            else
            {
                ret = new OperationStatus
                {
                    Status = false,
                    Message = "Invalid request"
                };
            }

            return ret;
        }
    }
}
