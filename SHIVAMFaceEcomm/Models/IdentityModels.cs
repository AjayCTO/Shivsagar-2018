using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace SHIVAMFaceEcomm.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

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
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            userIdentity.AddClaim(new System.Security.Claims.Claim("CustomerId", this.Id));
            // Add custom user claims here
            return userIdentity;
        }
    }

    public static class IdentityHelper
    {
        public static string GetCustomerId(this IIdentity identity)
        {
            var claimIdent = identity as ClaimsIdentity;
            return claimIdent != null
                && claimIdent.HasClaim(c => c.Type == "nameidentifier")
                ? claimIdent.FindFirst("nameidentifier").Value
                : string.Empty;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}