using AngularJSAuthentication.API.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

using AngularJSAuthentication.API.Models;
using AngularJSAuthentication.Models;

namespace AngularJSAuthentication.API
{
    public class AuthContext : IdentityDbContext<ApplicationUser>
    {
        public AuthContext()
            : base("AuthContext", throwIfV1Schema: false)
        {
     
        }

        //public DbSet<Merchant> Merchants { get; set; }
        //public DbSet<Customer> Customers { get; set; }
        //public DbSet<Service> Services { get; set; }
        //public DbSet<Task> Tasks { get; set; }
        //public DbSet<Employee> Employees { get; set; }
        //public DbSet<Client> Clients { get; set; }
        //public DbSet<RefreshToken> RefreshTokens { get; set; }
    }

}