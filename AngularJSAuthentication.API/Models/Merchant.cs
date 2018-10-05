using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AngularJSAuthentication.API.Models
{
    public class Merchant
    {
        public int Id { get; set; }

        [Display(Name = "Merchant Name")]
        [Required]
        public string Name { get; set; }


        [Display(Name = "Firm Name")]
        public string FirmName { get; set; }


        //[Display(Name = "Date Of Registration")]
        //public DateTime DateRegistration { get; set; }


        [Display(Name = "Phone Number")]
        [Required]
        public string ContactNumber { get; set; }


        [Display(Name = "Some Detail")]
        public string Description { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }


        public ICollection<Service> Services { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}