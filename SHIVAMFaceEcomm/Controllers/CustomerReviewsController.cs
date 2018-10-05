using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SHIVAMFaceEcomm.Models;

namespace SHIVAMFaceEcomm.Controllers
{
    public class CustomerReviewsController : ApiController
    {
        private SHIVAMECommerceDBNewEntities db = new SHIVAMECommerceDBNewEntities();

        // GET: api/CustomerReviews
        public IQueryable<CustomerReview> GetCustomerReviews()
        {
            return db.CustomerReviews;
        }


        public HttpResponseMessage GetCustomerReviews(int ProductId)
        {

            var result = db.CustomerReviews.Where(p => p.ProductId == ProductId).Select(p => new ReviewModel() { Email=p.Email, ProductId = p.ProductId, Name = p.Name, Review = p.Review });
            return Request.CreateResponse(HttpStatusCode.OK, result.ToList());
        }
        // GET: api/CustomerReviews/5
        [ResponseType(typeof(CustomerReview))]
        public IHttpActionResult GetCustomerReview(int id)
        {
            CustomerReview customerReview = db.CustomerReviews.Find(id);
            if (customerReview == null)
            {
                return NotFound();
            }

            return Ok(customerReview);
        }

        // PUT: api/CustomerReviews/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCustomerReview(int id, CustomerReview customerReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customerReview.Id)
            {
                return BadRequest();
            }

            db.Entry(customerReview).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerReviewExists(id))
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

        // POST: api/CustomerReviews
        [ResponseType(typeof(CustomerReview))]
        public IHttpActionResult PostCustomerReview(CustomerReview customerReview)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CustomerReviews.Add(customerReview);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = customerReview.Id }, customerReview);
        }

        // DELETE: api/CustomerReviews/5
        [ResponseType(typeof(CustomerReview))]
        public IHttpActionResult DeleteCustomerReview(int id)
        {
            CustomerReview customerReview = db.CustomerReviews.Find(id);
            if (customerReview == null)
            {
                return NotFound();
            }

            db.CustomerReviews.Remove(customerReview);
            db.SaveChanges();

            return Ok(customerReview);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerReviewExists(int id)
        {
            return db.CustomerReviews.Count(e => e.Id == id) > 0;
        }
    }
}