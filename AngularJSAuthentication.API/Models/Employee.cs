using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AngularJSAuthentication.API.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Display(Name = "Employee Name")]
        [Required]
        public string Name { get; set; }


        [Display(Name = "Address")]
        public string Address { get; set; }


        [Display(Name = "Phone Number")]
        [Required]
        public string ContactNumber { get; set; }


        [Display(Name = "Some Detail")]
        public string Description { get; set; }


        public Service ServicesAssigned { get; set; }
      
    }
}