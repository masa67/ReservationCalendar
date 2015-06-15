using ReservationCalendar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReservationCalendar.Controllers
{
    public class AbsCalendarTemplateController : Controller
    {
        // GET: AbsCalendarTemplate
        public ActionResult Index()
        {
            return View();
        }

        // POST: AbsCalendarTemplate/Edit
        [HttpPost]
        public ActionResult Edit(AbsCalendarTemplate absCalendarTemplate)
        {
            return null;
        }
    }
}