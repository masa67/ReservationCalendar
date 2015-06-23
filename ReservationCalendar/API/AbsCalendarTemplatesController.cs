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
    public class AbsCalendarLayersController : ApiController
    {
        private ReservationCalendarContext db = new ReservationCalendarContext();

        // GET: api/AbsCalendarLayers
        public IQueryable<AbsCalendarLayer> GetAbsCalendarLayers()
        {
            return db.AbsCalendarLayers;
        }

        // GET: api/AbsCalendarLayers/5
        [ResponseType(typeof(AbsCalendarLayer))]
        public async Task<IHttpActionResult> GetAbsCalendarLayer(int id)
        {
            AbsCalendarLayer absCalendarLayer = await db.AbsCalendarLayers.FindAsync(id);
            if (absCalendarLayer == null)
            {
                return NotFound();
            }

            return Ok(absCalendarLayer);
        }

        // PUT: api/AbsCalendarLayers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAbsCalendarLayer(int id, AbsCalendarLayer absCalendarLayer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != absCalendarLayer.ID)
            {
                return BadRequest();
            }

            db.Entry(absCalendarLayer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AbsCalendarLayerExists(id))
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

        // POST: api/AbsCalendarLayers
        [ResponseType(typeof(AbsCalendarLayer))]
        public async Task<IHttpActionResult> PostAbsCalendarLayer(AbsCalendarLayer absCalendarLayer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AbsCalendarLayers.Add(absCalendarLayer);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = absCalendarLayer.ID }, absCalendarLayer);
        }

        // DELETE: api/AbsCalendarLayers/5
        [ResponseType(typeof(AbsCalendarLayer))]
        public async Task<IHttpActionResult> DeleteAbsCalendarLayer(int id)
        {
            AbsCalendarLayer absCalendarLayer = await db.AbsCalendarLayers.FindAsync(id);
            if (absCalendarLayer == null)
            {
                return NotFound();
            }

            db.AbsCalendarLayers.Remove(absCalendarLayer);
            await db.SaveChangesAsync();

            return Ok(absCalendarLayer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AbsCalendarLayerExists(int id)
        {
            return db.AbsCalendarLayers.Count(e => e.ID == id) > 0;
        }
    }
     */
}