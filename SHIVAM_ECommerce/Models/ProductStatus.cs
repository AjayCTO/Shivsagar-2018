using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class ProductStatus : BaseClass
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage="Status name is required.")]
        [Display(Name="Status Name")]
        public string Name { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

    }
}