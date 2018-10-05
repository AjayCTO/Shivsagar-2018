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
    [RoutePrefix("/api/WishLists/{actionName}")]
    public class WishListsController : ApiController
    {
        private SHIVAMECommerceDBNewEntities db = new SHIVAMECommerceDBNewEntities();

        // GET: api/WishLists
        public List<WishListViewModel> GetWishLists(string UserID)
        {
            try
            {
                var _Customer = db.Customers.Where(x => x.UserID == UserID).FirstOrDefault();
                var _wishlistData = db.WishLists.Where(x => x.CustomerId == _Customer.Id).ToList();
                var _newlistwm = new List<WishListViewModel>();

                foreach (var _item in _wishlistData)
                {
                    var _productQuantity = db.ProductAttributeWithQuantities.Where(x => x.ProductId == _item.ProductId).ToList();
                    var _imagePath="";
                    
                    if(_productQuantity!=null && _productQuantity.Count()>0)
                    {
                        var _productIDs=_productQuantity.Select(x=>x.Id);
                        var _imagePathData=db.ProductImages.Where(x=>_productIDs.Contains(x.ProductQuantityId)).Select(x=>x.ImagePath);
                        
                        if(_imagePathData.Count()>0)
                        {
                            var _array=_imagePathData.ToArray();
                            _imagePath =_array[0];
                        }
                    }
                    _newlistwm.Add(new WishListViewModel() { Image = _imagePath, Id = _item.Id, ProductId = _item.ProductId, CustomerId = _item.CustomerId, ProductName = _item.Product.ProductName, ProductDescription = _item.Product.Description, ProductPrice = _productQuantity.Count() > 0 ? _productQuantity.FirstOrDefault().UnitPrice : 0 });
                }

                return _newlistwm;
            }
            catch (Exception)
            {

                throw;
            }
        }

       

        // POST: api/WishLists
        [ResponseType(typeof(WishList))]
        public APIResponse PostWishList(WishList wishList)
        {
            var _newError = new APIResponse();

            try
            {


                var _Customer = db.Customers.Where(x => x.UserID == wishList.UserID).FirstOrDefault();

                wishList.CustomerId = _Customer.Id;
                db.WishLists.Add(wishList);
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

        // DELETE: api/WishLists/5
        [HttpPost]
        [ActionName("DeleteWishList")]
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WishListExists(int id)
        {
            return db.WishLists.Count(e => e.Id == id) > 0;
        }
    }
}