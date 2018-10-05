using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHIVAMFaceEcomm.ViewModels
{
    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public string ProductName { get; set; }

        public string ProductImage { get; set; }

        public string ShippingDate { get; set; }

        public string SoldBy { get; set; }
    }
}