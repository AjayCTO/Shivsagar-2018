using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class Features : BaseClass
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}