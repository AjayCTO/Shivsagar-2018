using SHIVAMFaceEcomm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using Microsoft.AspNet.Identity;
using SHIVAMFaceEcomm.ViewModels;
namespace SHIVAMFaceEcomm.Controllers
{
    public class CustomerOdersDetailController : Controller
    {
        private SHIVAMECommerceDBNewEntities db = new SHIVAMECommerceDBNewEntities();
        //
        // GET: /CustomerOdersDetail/
        public ActionResult Index()
        {
            return View();
        }
 
        public ActionResult OrderdDetails()
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
            var _customer = db.Customers.Where(x => x.UserID == UserID.ToString()).FirstOrDefault();
            var orders = db.Orders.Where(x => x.CustomerID == _customer.Id).ToList();
            var v = (from a in orders select a);
            if (!string.IsNullOrEmpty(searchitem))
            {

                v = v.Where(b => b.OrderNumber.Contains(searchitem) || b.Description.Contains(searchitem) || b.OrderNumber.Contains(searchitem));
            }
            v = v.OrderBy(x => x.OrderDate);
            //SORT
            sortColumn = sortColumn == "Paid" ? "IsPaid" : sortColumn;
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.OrderNumber,  x.OrderDate, x.ShipDate,  Paid = x.IsPaid == true ? "Paid" : "UnPaid", x.TotalPrice, }) }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetOrderItems(int orderID)
        {
            try
            { 

           
                var _orderItems = db.OrderItems.Where(x => x.Orders_Id == orderID).ToList();
                var pid=_orderItems.Select(x=>x.ProductID);
                var productlist = db.Products.Where(p=>pid.Contains(p.Id)).ToList();
                var OrderItemslist = new List<OrderItemViewModel>();
                foreach (var _item in _orderItems.ToList())
                {
                    var obj = new OrderItemViewModel();
                    var _product = productlist.Where(x => x.Id == _item.ProductID).FirstOrDefault();
                    obj.ProductName = _product.ProductName;
                    obj.Quantity = _item.Quantity;
                    obj.UnitPrice = _item.UnitPrice;

                    OrderItemslist.Add(obj);
                    

                   
                }
                
              
                     return Json(OrderItemslist,JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                return Json(new { success = false, ex = ex.Message.ToString(), data = "" }, JsonRequestBehavior.AllowGet);
            }
         
        }

	}
}