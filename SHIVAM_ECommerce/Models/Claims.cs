using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class Claims : BaseClass
    {
        public string Title { get; set; }

        public string ClaimGroup { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public bool IsActive { get; set; }

        public string Role { get; set; }


    }
}