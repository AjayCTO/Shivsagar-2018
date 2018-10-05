using AngularJSAuthentication.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


namespace AngularJSAuthentication.API.Controllers
{
    public class CustomerReviewsController : ApiController
    {
        SHIVAMEcommerceDBEntities db = new SHIVAMEcommerceDBEntities();

        public HttpResponseMessage GetCustomerReviews(int ProductId)
        {
            try
            {


                var result = new
                {
                    data = db.CustomerReviews.Where(p => p.ProductId == ProductId).Select(p => new ReviewModel() { Email = p.Email, ProductId = p.ProductId, Name = p.Name, Review = p.Review, Rating=p.Rating.ToString()}),
                    Success = true,
                };

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    error = ex.InnerException.Message.ToString(),
                    data = ""
                };
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
        }

        [HttpPost]
        public  HttpResponseMessage PostCustomerReview(CustomerReview customerReview)
        {
            try
            {
                db.CustomerReviews.Add(customerReview);
                db.SaveChanges();
                var result = new
                {
                    Success = true,
                };
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    error = ex.InnerException.Message.ToString(),
                    success=false
                };
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }

        }
 
    }




}



