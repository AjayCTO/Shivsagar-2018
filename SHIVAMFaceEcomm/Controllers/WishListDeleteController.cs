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
    public class WishListDeleteController : ApiController
    {
        private SHIVAMECommerceDBNewEntities db = new SHIVAMECommerceDBNewEntities();

        // GET: api/WishLists

        // DELETE: api/WishLists/5
        [HttpPost]
        public APIResponse DeleteWishList(int id)
        {

            var _newError = new APIResponse();

            try
            {
                WishList wishList = db.WishLists.Find(id);
                db.WishLists.Remove(wishList);
                db.SaveChanges();
                _newError.ID = wishList.Id;
                _newError.Success = true;
                _newError.Ex = "";
                return _newError;
            }
            catch (Exception Ex)
            {

                _newError.ID = -1;
                _newError.Success = false;
                _newError.Ex = Ex.Message.ToString();
                return _newError;
            }

        }

    
    }
}