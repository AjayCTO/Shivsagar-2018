using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class FeaturedSupplier : BaseClass
    {
        [Required]
        public int SupplierID { get; set; }
        public string OfferMessage { get; set; }
        public string ImagePath { get; set; }

        [ForeignKey("SupplierID")]

        public virtual Supplier Supplier { get; set; }
    }
}