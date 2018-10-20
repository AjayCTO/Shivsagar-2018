using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AngularJSAuthentication.API.Models
{
    public class OrderViewModel
    {
        public List<OrderCartItem> OrderCartItem { get; set; }
        public OrderCustomerData OrderCustomerData { get; set; }
        public PaymentInfo PaymentInfo { get; set; }
        public decimal CartTotal { get; set; }
    }


    public class OrderCartItem
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public int productPrice { get; set; }
        public string productDescription { get; set; }
        public string image { get; set; }
        public int Quantity { get; set; }
        public int customerId { get; set; }
    }

    public class OrderCustomerData
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
    }

    public class PaymentInfo
    {
        public string CVV { get; set; }
        public string CardNumber { get; set; }
        public string NameOnCard { get; set; }
        public string ExpiryDate { get; set; }
        public string Zip { get; set; }
        public string CardType { get; set; }
    }
}