using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHIVAM_ECommerce.Models;
namespace SHIVAM_ECommerce.ViewModels
{
    public class CustomClaims
    {
        public int Id { get; set; }
        public bool IsChecked { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

    }

    public class ClaimsViewModel
    {
        public string Group { get; set; }

        public virtual List<Claims> AllClaims { get; set; }
    }
}
