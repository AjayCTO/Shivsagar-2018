using AngularJSAuthentication.API.Models;
using AngularJSAuthentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AngularJSAuthentication.API.Controllers
{
    public class CartController : ApiController
    {
        SHIVAMEcommerceDBEntities db = new SHIVAMEcommerceDBEntities();

        [HttpGet]
        //[ActionName("GetWishLists")]
        public List<CartListViewModel> GetCart(string UserName)
        {
            try
            {
                var UserId = db.AspNetUsers.FirstOrDefault(x => x.UserName == UserName).Id;             

                var _Customer = db.Customers.Where(x => x.UserID == UserId).FirstOrDefault();
                var _cartlistData = db.Carts.Where(x => x.CustomerId == _Customer.Id).ToList();
                var _newlistwm = new List<CartListViewModel>();

                foreach (var _item in _cartlistData)
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
                    _newlistwm.Add(new CartListViewModel() { Image = _imagePath, Id = _item.Id, ProductId = _item.ProductId, CustomerId = _item.CustomerId, ProductName = _item.Product.ProductName, ProductDescription = _item.Product.Description, ProductPrice = _productQuantity.Count() > 0 ? _productQuantity.FirstOrDefault().ProductPrice : 0 });
                }

                return _newlistwm;
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ActionName("PostWishList")]
        public HttpResponseMessage PostCart(Cart cart)
        {
            var UserId = db.AspNetUsers.FirstOrDefault(x => x.UserName == cart.UserID).Id;
            try
            {
                var _Customer = db.Customers.Where(x => x.UserID == UserId).FirstOrDefault();
                cart.CustomerId = _Customer.Id;
                db.Carts.Add(cart);
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

        [HttpDelete]
        //[ActionName("DeleteFromCart")]
        public HttpResponseMessage DeleteFromCart(int id , string UserName)
        {
            var UserId = db.AspNetUsers.FirstOrDefault(x => x.UserName == UserName).Id;     
            try
            {
                var _Customer = db.Customers.Where(x => x.UserID == UserId).FirstOrDefault();

                var cart = db.Carts.Find(id);
                db.Carts.Remove(cart);
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
