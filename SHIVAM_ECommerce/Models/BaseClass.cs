using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SHIVAM_ECommerce.Models

{
    public class BaseClass
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public int Sort { get; set; }
        public string Description { get; set; }

        public string Notes { get; set; }


    }
}