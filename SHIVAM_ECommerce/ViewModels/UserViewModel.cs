using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.ViewModels
{
    public class UserViewModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }
        public List<CustomClaims> Claims { get; set; }
    }
    public class SupplierVM
    {
       
  
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
        public int PlanID { get; set; }

        public int ProductCount { get; set; }
        public int UserCount { get; set; }

        [Required]
        [MinLength(8)]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(7)]
        public string Password { get; set; }
    }
    public class UserRoleViewModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }
        public List<CustomRoles> Roles { get; set; }
    }
}