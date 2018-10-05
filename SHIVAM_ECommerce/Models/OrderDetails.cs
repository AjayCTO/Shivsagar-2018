using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class OrderDetails:BaseClass
    {
       [Required(ErrorMessage = "Order Number is Required.")]
        public string OrderNumber { get; set; }
        [Required(ErrorMessage = "Order Price is Required.")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Quantity is Required.")]
        public int Quantity { get; set; }
        public bool? Discount { get; set; }
        public DateTime ShipDate { get; set; }
        public DateTime BillDate { get; set; }
        [Required(ErrorMessage = "Total is Required.")]
        public decimal Total { get; set; }
        public int OrderID { get; set; }


        [ForeignKey("OrderID")]

        public virtual Orders Order { get; set; }
    }
}