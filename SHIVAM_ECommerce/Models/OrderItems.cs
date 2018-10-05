using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class OrderItems : BaseClass
    {
        [ForeignKey("ProductAttributeWithQuantity")]
        public int ProductID { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        public decimal Discount { get; set; }


        public virtual ProductAttributeWithQuantity ProductAttributeWithQuantity { get; set; }

        

        public int SupplierID { get; set; }

        
  
        public int Orders_Id { get; set; }

        [ForeignKey("Orders_Id")]
        public virtual Orders Order { get; set; }
    }
}