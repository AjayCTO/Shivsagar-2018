using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SHIVAM_ECommerce.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public string resetPasswordToken { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string Email { get; set; }

        public bool  EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public bool LockoutEnabled { get; set; }
        public int? Sort { get; set; }
        public int AccessFailedCount { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            userIdentity.AddClaim(new System.Security.Claims.Claim("CustomerId", this.Id));
            // Add custom user claims here
            return userIdentity;
        }
       
    }
    public class ApplicationUserClaim : IdentityUserClaim
    {

        public int? ClaimID { get; set; }
        public bool IsActive { get; set; }
        public string DisplayLabel { get; set; }

        public string UserId { get; set; }

        [ForeignKey("ClaimID")]
        public virtual Claims claims { get; set; }

    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Product> Products { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Category> Cateogries { get; set; }

        public DbSet<SupplierCategory> SupplierCateogries { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<emailrecord> EmailRecord { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Tax> Taxes { get; set; }

        public DbSet<Plans> Plans { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }

        public DbSet<Claims> Claims { get; set; }

        public DbSet<Cart> Cart { get; set; }

        public DbSet<ApplicationUserClaim> AspNetUserClaims { get; set; }

        public DbSet<ProductAttributes> ProductAttributes { get; set; }
        public DbSet<ProductAttributesRelation> ProductAttributesRelation { get; set; }
        public DbSet<ProductAttributeWithQuantity> ProductAttributeWithQuantity { get; set; }
        
        public DbSet<UnitOfMeasures> UnitOfMeasures { get; set; }

        public DbSet<Manufacturer> Manufacturers { get; set; }

        public DbSet<WishList> Wishlist { get; set; }

        public DbSet<CustomerReviews> Reviews { get; set; }
        public DbSet<ProductStatus> ProductStatus { get; set; }

        public DbSet<Features> Features { get; set; }

        public DbSet<PlanFeatures> PlanFeatures { get; set; }


        public DbSet<FeaturedSupplier> FeaturedSuppliers { get; set; }
        public System.Data.Entity.DbSet<SHIVAM_ECommerce.Models.AllProductImages> AllProductImages { get; set; }

        public System.Data.Entity.DbSet<SHIVAM_ECommerce.Models.ThreshHold> ThreshHolds { get; set; }


        public DbSet<EmailSender> EmailSenders { get; set; }     


    }
}