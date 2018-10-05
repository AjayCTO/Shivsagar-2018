using AngularJSAuthentication.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AngularJSAuthentication.API.Controllers
{
   
    public class WishlistDeleteController : ApiController
    {
        SHIVAMEcommerceDBEntities db = new SHIVAMEcommerceDBEntities();

        [HttpPost]
        public HttpResponseMessage DeleteWishList(int id)
        {
            try
            {
                WishList wishList = db.WishLists.Find(id);
                db.WishLists.Remove(wishList);
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
                    success = false
                };
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }

        }

    }
}
