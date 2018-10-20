using AngularJSAuthentication.API.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AngularJSAuthentication.Models
{

    public class ApplicationUser : IdentityUser
    {
        [NotMapped]
        public string UserId { get; set; }
        [NotMapped]
        public string resetPasswordToken { get; set; }
        [NotMapped]
        public DateTime? CreatedDate { get; set; }
        [NotMapped]
        public DateTime? UpdatedDate { get; set; }
        [NotMapped]

  
        public int? Sort { get; set; }
        [NotMapped]
        public string Discriminator { get; set; }
      
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            userIdentity.AddClaim(new System.Security.Claims.Claim("CustomerId", this.Id));
            // Add custom user claims here
            return userIdentity;
        }
    }


    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    public ApplicationDbContext()
    //        : base("DefaultConnection", throwIfV1Schema: false)
    //    {
    //    }

    //    public static ApplicationDbContext Create()
    //    {
    //        return new ApplicationDbContext();
    //    }
    //}

    public class CategoryViewModel
    {
        
    
        public int Id { get; set; }
        public string CategoryName { get; set; }
         
        public Nullable<int> ParentCategory { get; set; }
        public string CategoryImage { get; set; }
        public string Description { get; set; }

        public List<CategoryViewModel> Categories1 { get; set; }
        
    }

    public class WishListItems
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }
        public Product Product { get; set; }
    }

    public class WishListViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }

        public string ProductDescription { get; set; }

        public string Image { get; set; }

        public int CustomerId { get; set; }
    }

    public class CartListViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }

        public string ProductDescription { get; set; }

        public string Image { get; set; }

        public int CustomerId { get; set; }
    }


    public class ContactViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string EmailAddress { get; set; }
        public string Message { get; set; }
    }

}
