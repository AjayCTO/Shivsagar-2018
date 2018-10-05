using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.ViewModels
{
    public class CurrentUserContext
    {
    
        public int SupplierID { get; set; }
        public string UserID { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Roles { get; set; }

        public int ProductCount { get; set; }

        public int UserCount { get; set; }

        public DateTime PlanEndDate { get; set; }
        public bool IsSuperAdmin { get; set; }
        public int PlanId { get; set; }
        public string CompanyName { get; set; }
    }
}