using AngularJSAuthentication.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AngularJSAuthentication.API.Controllers
{
    public class CustomerOrdersController : ApiController
    {
        SHIVAMEcommerceDBEntities db = new SHIVAMEcommerceDBEntities();
        [HttpPost]
        [ActionName("PostOrder")]
        public HttpResponseMessage PostOrder(OrderViewModel OrderViewModel)
        {

            try
            {
                var Customer = db.Customers.FirstOrDefault(x => x.AspNetUser.UserName == OrderViewModel.OrderCustomerData.userName);

                Random rnd = new Random();
                int ordernumber = rnd.Next(1, 1000);

                Order order = new Order();
                order.OrderNumber = ordernumber.ToString();
                order.OrderDate = DateTime.Now;
                order.ShipDate = DateTime.Now; // Adjust ship date accourding to the system requirement later.
                order.RequiredDate = DateTime.Now; // Get more info about this field.
                order.IsPaid = false; // Get infor of this field from the payment information.
                order.TimeStamp = null; // ??
                order.Isdeleted = null; //??
                order.TransactionStatus = "Ordre Placed";
                order.CustomerID = Customer.Id;
                order.OrderStatusID = 1; // 1 for Order placed status.
                order.TotalDiscount = 0;
                order.TotalPrice = OrderViewModel.CartTotal;
                order.CreatedDate = DateTime.Now;
                order.UpdatedDate = DateTime.Now;
                order.Sort = 0;
                order.Description = "Order Placed";
                order.Notes = "Order Placed";

                db.Orders.Add(order);
                db.SaveChanges();


                foreach (var item in OrderViewModel.OrderCartItem)
                {
                    OrderItem orderItem = new OrderItem();

                    var supplierId = db.Products.FirstOrDefault(x => x.Id == item.productId).SupplierID;

                    orderItem.ProductID = item.productId;
                    orderItem.UnitPrice = item.productPrice;
                    orderItem.Quantity = item.Quantity;
                    orderItem.TotalPrice = item.Quantity * item.productPrice;
                    orderItem.Discount = 0;
                    orderItem.SupplierID = supplierId;
                    orderItem.Orders_Id = order.Id;
                    orderItem.CreatedDate = DateTime.Now;
                    orderItem.UpdatedDate = DateTime.Now;
                    orderItem.Sort = 0;
                    orderItem.Description = item.productDescription;
                    orderItem.Notes = item.productDescription;

                    db.OrderItems.Add(orderItem);

                }
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



        [HttpGet]
           [ActionName("GetOrder")]
         public HttpResponseMessage GetOrder(string UserName)
        {


            try
            {

                var UserId = db.AspNetUsers.FirstOrDefault(x => x.UserName == UserName).Id;

                var _customer = db.Customers.Where(x => x.UserID == UserId.ToString()).FirstOrDefault();
                var orders = db.Orders.Where(x => x.CustomerID == _customer.Id).ToList();
                var result = new
                {
                    Success = true,
                    data = orders.Select(x => new { OrderNumber=x.OrderNumber, Id=x.Id,Orderdate=x.OrderDate,ShippingDate=x.ShipDate,paymentstatus=x.IsPaid, total=x.TotalPrice }).ToList()
                };
                return Request.CreateResponse(HttpStatusCode.OK,result);
          
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

        [HttpGet]
        [ActionName("GetOrderItem")]
        public HttpResponseMessage GetOrderItem(int orderID)
        {

            try
            {
                var _orderItems = db.OrderItems.Where(x => x.Orders_Id == orderID).ToList();
                var pid = _orderItems.Select(x => x.ProductID);
                var productlist = db.Products.Where(p => pid.Contains(p.Id)).ToList();
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
                var result = new
                {
                    Success = true,
                    data = OrderItemslist
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
