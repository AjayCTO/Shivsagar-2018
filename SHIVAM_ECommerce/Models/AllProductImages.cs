using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class AllProductImages : BaseClass
    {
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public string UserID { get; set; }

        public int? SupplierID { get; set; }

        [ForeignKey("SupplierID")]

        public virtual Supplier Supplier { get; set; }
    }
}