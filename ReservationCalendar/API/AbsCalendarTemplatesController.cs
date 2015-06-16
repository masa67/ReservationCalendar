using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ReservationCalendar.DAL;
using ReservationCalendar.Models;

namespace ReservationCalendar.API
{
    /*
    public class AbsCalendarTemplatesController : ApiController
    {
        private ReservationCalendarContext db = new ReservationCalendarContext();

        // GET: api/AbsCalendarTemplates
        public IQueryable<AbsCalendarTemplate> GetAbsCalendarTemplates()
        {
            return db.AbsCalendarTemplates;
        }

        // GET: api/AbsCalendarTemplates/5
        [ResponseType(typeof(AbsCalendarTemplate))]
        public async Task<IHttpActionResult> GetAbsCalendarTemplate(int id)
        {
            AbsCalendarTemplate absCalendarTemplate = await db.AbsCalendarTemplates.FindAsync(id);
            if (absCalendarTemplate == null)
            {
                return NotFound();
            }

            return Ok(absCalendarTemplate);
        }

        // PUT: api/AbsCalendarTemplates/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAbsCalendarTemplate(int id, AbsCalendarTemplate absCalendarTemplate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != absCalendarTemplate.ID)
            {
                return BadRequest();
            }

            db.Entry(absCalendarTemplate).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AbsCalendarTemplateExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/AbsCalendarTemplates
        [ResponseType(typeof(AbsCalendarTemplate))]
        public async Task<IHttpActionResult> PostAbsCalendarTemplate(AbsCalendarTemplate absCalendarTemplate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AbsCalendarTemplates.Add(absCalendarTemplate);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = absCalendarTemplate.ID }, absCalendarTemplate);
        }

        // DELETE: api/AbsCalendarTemplates/5
        [ResponseType(typeof(AbsCalendarTemplate))]
        public async Task<IHttpActionResult> DeleteAbsCalendarTemplate(int id)
        {
            AbsCalendarTemplate absCalendarTemplate = await db.AbsCalendarTemplates.FindAsync(id);
            if (absCalendarTemplate == null)
            {
                return NotFound();
            }

            db.AbsCalendarTemplates.Remove(absCalendarTemplate);
            await db.SaveChangesAsync();

            return Ok(absCalendarTemplate);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AbsCalendarTemplateExists(int id)
        {
            return db.AbsCalendarTemplates.Count(e => e.ID == id) > 0;
        }
    }
     */
}