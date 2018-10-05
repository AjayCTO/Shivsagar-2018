using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class Supplier : BaseClass
    {

        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string URL { get; set; }
        public string Logo { get; set; }
        public string SupplierType { get; set; }

        public int ParentSupplierID { get; set; }
        public string RegisteredByID { get; set; }
        public string UserID { get; set; }
        public int PlanID { get; set; }

        public DateTime PlanStartDate { get; set; }

        public DateTime PlanEndDate { get; set; }

        public int ProductCount { get; set; }
        public int UserCount { get; set; }


        [NotMapped]
        [Required]
        [MinLength(8)]
        public string UserName { get; set; }

        [NotMapped]
        [Required]
        [MinLength(7)]
        public string Password { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("PlanID")]
        public virtual Plans Plan { get; set; }

        //public virtual List<Manufacturer> Manufacturers { get; set; }
        public virtual List<Category> Categories { get; set; }
        public virtual List<Product> Products { get; set; }
    }

    
}