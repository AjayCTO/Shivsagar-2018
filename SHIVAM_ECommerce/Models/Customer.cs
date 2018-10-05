using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class Customer : BaseClass
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string CreditCard { get; set; }
        public string CreditCardType { get; set; }
        public string CardExpMo { get; set; }
        public string CardExpYr { get; set; }

        [NotMapped]
        [Required]
        [MinLength(8)]
        public string UserName { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [NotMapped]
        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string UserID { get; set; }


        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        public ICollection<Orders> Orders { get; set; }
    }
}