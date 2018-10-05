using AngularJSAuthentication.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using System.Web.Http;
using AngularJSAuthentication.Models;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Newtonsoft.Json;
namespace AngularJSAuthentication.API.Controllers
{
   [Route("api/CustomerWishlist/{action}")]
 
 
    public class CustomerWishlistController : ApiController
    {
        SHIVAMEcommerceDBEntities db = new SHIVAMEcommerceDBEntities();

        [HttpGet]
        [ActionName("GetWishLists")]
        public List<WishListViewModel> GetWishLists(string UserName)
        {

    
            try
            {
               
                var UserId = db.AspNetUsers.FirstOrDefault(x => x.UserName == UserName).Id;

             

                var _Customer = db.Customers.Where(x => x.UserID == UserId).FirstOrDefault();
                var _wishlistData = db.WishLists.Where(x => x.CustomerId == _Customer.Id).ToList();
                var _newlistwm = new List<WishListViewModel>();

                foreach (var _item in _wishlistData)
                {
                    var _productQuantity = db.ProductAttributeWithQuantities.Where(x => x.ProductId == _item.ProductId).ToList();
                    var _imagePath = "";

                    if (_productQuantity != null && _productQuantity.Count() > 0)
                    {
                        var _productIDs = _productQuantity.Select(x => x.Id);
                        var _imagePathData = db.ProductImages.Where(x => _productIDs.Contains(x.ProductQuantityId)).Select(x => x.ImagePath);

                        if (_imagePathData.Count() > 0)
                        {
                            var _array = _imagePathData.ToArray();
                            _imagePath = _array[0];
                        }
                    }
                    _newlistwm.Add(new WishListViewModel() { Image = _imagePath, Id = _item.Id, ProductId = _item.ProductId, CustomerId = _item.CustomerId, ProductName = _item.Product.ProductName, ProductDescription = _item.Product.Description, ProductPrice = _productQuantity.Count() > 0 ? _productQuantity.FirstOrDefault().UnitPrice : 0 });
                }

                return  _newlistwm;
            }
            catch (Exception)
            {

                throw;
            }
        }

       
        [HttpGet]
        [ActionName("GetCustomerinfo")]
        public HttpResponseMessage GetCustomerinfo(string UserName)
        {
            try{
                var UserId = db.AspNetUsers.FirstOrDefault(x => x.UserName == UserName).Id;
                var _Customer = db.Customers.Where(x => x.UserID == UserId).Select(p => new {p.Phone,p.LastName,p.FirstName,p.Email});
              
            var result = new
            {
                Success = true,
                Data = _Customer

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
        [ActionName("PostWishList")]
        public HttpResponseMessage  PostWishList(WishList wishList)
        {
            var UserId = db.AspNetUsers.FirstOrDefault(x => x.UserName == wishList.UserID).Id;
            try
            {
                var _Customer = db.Customers.Where(x => x.UserID == UserId).FirstOrDefault();
                wishList.CustomerId = _Customer.Id;
                db.WishLists.Add(wishList);
                db.SaveChanges();
                var result = new
                {
                    success = true,
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
