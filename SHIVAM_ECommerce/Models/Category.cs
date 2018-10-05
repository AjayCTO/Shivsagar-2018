using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class Category :BaseClass
    {

        [Required]
        public string  CategoryName { get; set; }

        public bool IsActive { get; set; }

        public int? ParentCategory { get; set; }

        [ForeignKey("ParentCategory")]
        public virtual Category Categories { get; set; }


        public bool IsTopCategory { get; set; }

        public string CategoryImage { get; set; }

        //public int SupplierID { get; set; } 

        //[ForeignKey("SupplierID")]

        //public virtual Supplier Supplier { get; set; }
    }
}