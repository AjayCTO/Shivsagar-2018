using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class Orders : BaseClass
    {
        [Required(ErrorMessage = "Order Number is Required.")]
        public string OrderNumber { get; set; }
        [Required(ErrorMessage = "Order Date is Required.")]
        public DateTime OrderDate { get; set; }
        public DateTime ShipDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public bool? IsPaid { get; set; }
        public string TimeStamp { get; set; }
        public bool? Isdeleted { get; set; }
        public string TransactionStatus { get; set; }
        public int CustomerID { get; set; }
        public int OrderStatusID { get; set; }

        public decimal TotalDiscount { get; set; }


        public decimal TotalPrice{ get; set; }
        [ForeignKey("CustomerID")]
        public virtual Customer customer { get; set; }

        public ICollection<OrderItems> OrderItems { get; set; }


        [ForeignKey("OrderStatusID")]
        public virtual OrderStatus status { get; set; }
    }
}