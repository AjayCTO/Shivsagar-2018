using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class Tax : BaseClass
    {
        #region Properties
        public string Code { get; set; }
        [Required(ErrorMessage = "Tax Name is Required.")]
        public string Name { get; set; }
        public string Descripton { get; set; }
        [Required(ErrorMessage = "Tax Category is Required.")]
        public string Category { get; set; }
        [Required(ErrorMessage = "Tax Rate is Required.")]
        public double Rate { get; set; }
        #endregion
    }
}