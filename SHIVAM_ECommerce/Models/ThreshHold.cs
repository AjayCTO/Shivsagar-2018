using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class ThreshHold
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Product Id")]
        public int ProductId { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Display(Name="Low Quantity Threshold")]
        public int LowQuantityThresHold { get; set; }
        [Display(Name = "High Quantity Threshold")]
        public int HighQuantityThresHold { get; set; }

    }
}