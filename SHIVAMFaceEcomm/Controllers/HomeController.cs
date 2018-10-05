using SHIVAMFaceEcomm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using SHIVAMFaceEcomm.Service;
using System.Configuration;
using System.IO;
using SHIVAMFaceEcomm.ViewModels;

namespace SHIVAMFaceEcomm.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        SHIVAMECommerceDBNewEntities db = new SHIVAMECommerceDBNewEntities();
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
        public ActionResult AddNewCustomer()
        {
            return View();
        }

        public ActionResult Products(string ProductName, string CategoryID)
        {
            ViewBag.ProductName = !string.IsNullOrEmpty(ProductName) ? ProductName : "";
            ViewBag.CategoryID = !string.IsNullOrEmpty(CategoryID) ? CategoryID : ""; ;

            return View();
        }
        public ActionResult MyProducts()
        {

            return View();
        }

        public ActionResult Checkout()
        {
            return View();
        }
        public ActionResult DeleteWishList(int id)
        {


            try
            {
                WishList wishList = db.WishLists.Find(id);
                db.WishLists.Remove(wishList);
                db.SaveChanges();
                return Json(new { Success = true, Ex = "", ID = wishList.Id }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {

                return Json(new { Success = false, Ex = Ex.Message.ToString(), ID = -1 }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult CustomerWishList(Guid CustomerId)
        {
            return View(CustomerId);
        }

        public ActionResult LoadCustomerWishList()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchitem = Request["search[value]"];
            //Find Order Column
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            //var _supplierID = !string.IsNullOrEmpty(Request["SupplierID"]) ? Convert.ToInt16(Request["SupplierID"]) : CurrentUserData.SupplierID;

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var UserID = User.Identity.GetUserId();
            var _Customer = db.Customers.Where(x => x.UserID == UserID).FirstOrDefault();
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

            var v = (from a in _newlistwm select a);
            if (!string.IsNullOrEmpty(searchitem))
            {

                v = v.Where(b => b.ProductName.Contains(searchitem) || b.ProductDescription.Contains(searchitem));
            }

            //SORT


            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.ProductId, x.Image, x.ProductName, x.ProductPrice, }) }, JsonRequestBehavior.AllowGet);
        }









        public ActionResult ProductDetail(int ProductId)
        {
            ProductDetail productDetail = new ProductDetail();
            productDetail.Product = new Product();
            productDetail.ProductQuantityID = ProductId;

            var _ProductData = db.ProductAttributeWithQuantities.Where(p => p.Id == ProductId).FirstOrDefault();
            productDetail.Product = db.Products.Where(p => p.Id == _ProductData.ProductId).FirstOrDefault();
            productDetail.Attributes = new List<ProductDetailAttributes>();
            productDetail.Attributes = db.ProductValues_view.Where(p => p.productId == _ProductData.ProductId).Select(p => new ProductDetailAttributes() { AttributeName = p.AttributeName, AttributeValue = p.AttributeValue, Cost = p.Cost, ImageName = p.ImageName, ImagePath = p.ImagePath, Quantity = p.Quantity, ProductQuantityId = p.ProductQuantityID }).ToList();


            return View(productDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactViewModel contact)
        {
            if (ModelState.IsValid)
            {
                string CustomerName = contact.Name;
                string CustomerMessage = contact.Message;
                string CustomerEmail = contact.EmailAddress;
                string adminemail = ConfigurationManager.AppSettings["adminemail"].ToString();
                string admintemple = ConfigurationManager.AppSettings["admintemple"].ToString();
                string subject = ConfigurationManager.AppSettings["subject"].ToString();
                string Thankyou = ConfigurationManager.AppSettings["Thankyou"].ToString();
                string CustomerContactus = ConfigurationManager.AppSettings["CustomerContactus"].ToString();
                string AdminUserName = ConfigurationManager.AppSettings["adminusername"].ToString();
                var sendmail = new ContactEmail();
                //for cutomer
                sendmail.ContactEmailSend(CustomerEmail, Thankyou, "", CustomerName, " ", " ");
                //for Admin
                sendmail.ContactEmailSend(adminemail, CustomerContactus, subject, "Admin", CustomerEmail, CustomerMessage);
                return RedirectToAction("Contact", "Home");
            }
            return View(contact);
        }
        [HttpGet]
        public ActionResult GetRelatedProducts(int productID)
        {
            try
            {

                var _product = db.ProductAttributeWithQuantities.Where(p => p.Id == productID).FirstOrDefault();
                var _RelatedProducts = db.ProductAttributeWithQuantities.Where(x => x.Product.CateogryID == _product.Product.CateogryID && x.Product.ManuFacturerID == _product.Product.ManuFacturerID).Take(10).ToList();
                _RelatedProducts = _RelatedProducts == null ? new List<ProductAttributeWithQuantity>() : _RelatedProducts;
                return Json(new { success=true, error = "", data = _RelatedProducts.Select(x => new { ProductName = x.Product.ProductName, ProductID = x.ProductId, x.ProductPrice }).ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new {success=false, error = ex.InnerException.Message.ToString(), data = "" }, JsonRequestBehavior.AllowGet);
            }


        }

    }
}
