using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class Manufacturer:BaseClass
    {
        public string Code { get; set; }
        [Required(ErrorMessage = "Manufacturer Name is required")]
        public string Name { get; set; }

        public int SupplierID { get; set; }

        //[ForeignKey("SupplierID")]

        //public virtual Supplier Supplier { get; set; }
    }
}