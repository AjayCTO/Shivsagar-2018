namespace SHIVAM_ECommerce.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using SHIVAM_ECommerce.Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SHIVAM_ECommerce.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "SHIVAM_ECommerce.Models.ApplicationDbContext";
        }

        protected override void Seed(SHIVAM_ECommerce.Models.ApplicationDbContext context)
        {

            // Delete all stored procs, views
            //foreach (var file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sql\\Seed"), "*.sql"))
            //{
            //    context.Database.ExecuteSqlCommand(File.ReadAllText(file), new object[0]);
            //}

            // Add Stored Procedures and Views.
            if (!context.Roles.Any(r => r.Name == "superadmin"))
            {
                foreach (var file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin", string.Empty), "Sql"), "*.sql").OrderBy(x => x))
                {
                    context.Database.ExecuteSqlCommand(File.ReadAllText(file), new object[0]);
                }
            }


            if (!context.Roles.Any(r => r.Name == "superadmin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "superadmin" };

                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "Supplier"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Supplier" };

                manager.Create(role);


                var listOfClaims = new List<Claims>
                {                     
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Manufacturers",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "Supplier",Notes= "Can See Manufacturer",Role = "Supplier",Title="Manufacturer",ClaimGroup="Manufacturer" },                    
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Product/GetAllProducts",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "Supplier",Notes= "Can See Products",Role = "Supplier",Title="Products",ClaimGroup="Products"  },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Customer",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "Supplier",Notes= "Can See Customer",Role = "Supplier",Title="Customer",ClaimGroup="Customer"  },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/ProductAttributes/AddAttributesForSupplier",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "Supplier",Notes= "Can See Product Attr For Supplier",Role = "Supplier",Title="Attributes add for products",ClaimGroup="Products"  },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Order",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "Supplier",Notes= "Can See Orders",Role = "Supplier",Title="Orders",ClaimGroup="Customer"  },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/AllImages",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "Supplier",Notes= "Can See Images",Role = "Supplier",Title="Images",ClaimGroup="Images"},
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/SupplierUser",IsActive=false,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "Supplier",Notes= "Can See Supplier User",Role = "Supplier",Title="SupplierUsers",ClaimGroup="SupplierUsers"  },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/ManageClaims",IsActive=false,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "Supplier",Notes= "Can See ManageClaims",Role = "Supplier",Title="ManageClaims",ClaimGroup="ManageClaims"  },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/ManageUsers",IsActive=false,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "Supplier",Notes= "Can See ManageUsers",Role = "Supplier",Title="ManageUsers",ClaimGroup="ManageUsers"},
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/EmailSenders",IsActive=true,CreatedDate= DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "Supplier",Notes= "Can See Email",Role = "Supplier",Title="Messanger",ClaimGroup="Messanger"},
                };

                listOfClaims.ForEach(u => context.Claims.Add(u));
                context.SaveChanges();
               

            }


            if (!context.Features.Any(r => r.Title == "Email Service"))
            {
                var listoffeatures = new List<Features>{
                    new Features{Title = "Email Service",Price = 1000,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "Email Service", Notes = "Email Service" },
                    new Features{Title = "SMS Service",Price = 1000,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SMS Service", Notes = "SMS Service" },
                };

                listoffeatures.ForEach(u => context.Features.Add(u));
                context.SaveChanges();
            }



            if (!context.Roles.Any(r => r.Name == "SupplierUser"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "SupplierUser" };

                manager.Create(role);

                var listOfClaims = new List<Claims>
                {                     
                     new Claims{ClaimType = "URL",ClaimValue = "URL:/Manufacturers",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SupplierUser",Notes= "Can See Manufacturer",Role = "SupplierUser",Title="Manufacturer",ClaimGroup="Manufacturer"  },                    
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Product/GetAllProducts",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SupplierUser",Notes= "Can See Products",Role = "SupplierUser",Title="Products",ClaimGroup="Products"},
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Customer",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SupplierUser",Notes= "Can See Customer",Role = "SupplierUser",Title="Customer",ClaimGroup="Customer"  },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/ProductAttributes/AddAttributesForSupplier",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SupplierUser",Notes= "Can See Product Attr For Supplier",Role = "SupplierUser",Title="Attributes add for products",ClaimGroup="Products"  },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Order",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SupplierUser",Notes= "Can See Orders",Role = "SupplierUser",Title="Orders",ClaimGroup="Customer"  },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/AllImages",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SupplierUser",Notes= "Can See Images",Role = "SupplierUser",Title="Images",ClaimGroup="Images"},
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/EmailSenders",IsActive=true,CreatedDate= DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SupplierUser",Notes= "Can See Email",Role = "SupplierUser",Title="Messanger",ClaimGroup="Messanger"},   
                };

                listOfClaims.ForEach(u => context.Claims.Add(u));
                context.SaveChanges();

            }

            if (!context.Roles.Any(r => r.Name == "Customer"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Customer" };

                manager.Create(role);
            }


            //if (!context.Plans.Any(p => p.PlanName == "SilverPackage"))
            //{
            //    var Plans = new List<Plans>
            //    {
            //        new Plans {PlanName = "PlatinumPackage", Plancode = "#111",PlanFrequency="Monthly",MonthlyPrice= 100,YearlyPrice=1000,TotalYearlyPrice=2500,TotalMonthlyPrice=2100,ProductBucketCount=1000,IsActive=true,UserBucketCount=1000,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 1 ,Description="Dummy plan created for demonstration purposes",Notes="Dummy plan created for demonstration purposes" },
            //    };

            //    Plans.ForEach(p => context.Plans.Add(p));
            //    context.SaveChanges();
            //}


            if (!context.Users.Any(u => u.UserName == "Admin"))
            {

                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "Admin" };

                manager.Create(user, "Shivam@123");
                manager.AddToRole(user.Id, "superadmin");
                var AdminProfile = new List<AdminProfile>
                {
                new AdminProfile{ Id=-1,FirstName="Ajay",LastName="Paliwal",CompanyName="ShivamItConsultancy", Email="ajaypal.mca@outlook.com",Address="bus stand",City="Nathdwara",Country="India",Logo=null,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now,Sort=1, Phone="8386911436",PostalCode="313301",State="Rajasthan"},
                };
                AdminProfile.ForEach(p => context.Adminprofile.Add(p));
                context.SaveChanges();

                var listOfClaims = new List<Claims>
                {
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Category",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See Category",Role = "SuperAdmin",Title="Category",ClaimGroup="Category" },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Supplier",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See Supplier",Role = "SuperAdmin",Title="Supplier",ClaimGroup="Supplier" },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Plan",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See Plan",Role = "SuperAdmin",Title="Plan",ClaimGroup="Plan" },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Manufacturers",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See Manufacturer",Role = "SuperAdmin",Title="Manufacturer",ClaimGroup="Manufacturer"},
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Product/GetAllProducts",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See Products",Role = "SuperAdmin",Title="Products",ClaimGroup="Products"},
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/ProductStatus",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See Product Status",Role = "SuperAdmin",Title="Products Status",ClaimGroup="Products" },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/UnitOfMeasures",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See UOM",Role = "SuperAdmin",Title="Unit of Measure",ClaimGroup="Products" },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/ProductAttributes",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can see Product Attribute",Role = "SuperAdmin",Title="Product Attributes",ClaimGroup="Products" },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/ProductAttributes/AddAttributesForSupplier",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See Product Attr For Supplier",Role = "SuperAdmin",Title="Add Attributes For Supplier",ClaimGroup="Products" },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Customer",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See Customer",Role = "SuperAdmin",Title="Customer",ClaimGroup="Customer" },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Order",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See Orders",Role = "SuperAdmin",Title="Orders",ClaimGroup="Customer"},
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/SupplierUser",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See SupplierUser",Role = "SuperAdmin",Title="Supplier User",ClaimGroup="Supplier" },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Account/Manage",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See Settings",Role = "SuperAdmin",Title="Settings",ClaimGroup="Settings" },
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/ManageClaims",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See Claims",Role = "SuperAdmin",Title="Manage Claim",ClaimGroup="Settings"},
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/AllImages",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See Images",Role = "SuperAdmin",Title="Images",ClaimGroup="Images"},
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/Features",IsActive=true,CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See Feature",Role = "SuperAdmin",Title="Feature",ClaimGroup="Feature"},
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/ManageUsers",IsActive=true,CreatedDate= DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See ManageUsers",Role = "SuperAdmin",Title="Manage Users",ClaimGroup="Settings"},
                    new Claims{ClaimType = "URL",ClaimValue = "URL:/EmailSenders",IsActive=true,CreatedDate= DateTime.Now,UpdatedDate = DateTime.Now,Sort = 0,Description = "SuperAdmin",Notes= "Can See Email",Role = "SuperAdmin",Title="Messanger",ClaimGroup="Messanger"},
                 };

                listOfClaims.ForEach(u => context.Claims.Add(u));
                context.SaveChanges();

                var _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    foreach (var claim in listOfClaims)
                    {
                        String query = "INSERT INTO [dbo].[AspNetUserClaims]([ClaimType],[ClaimValue],[ClaimID],[IsActive],[DisplayLabel],[Discriminator],[User_Id]) VALUES(@ClaimType,@ClaimValue,@ClaimID,@IsActive,@DisplayLabel,@Discriminator,@User_Id)";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@ClaimType", claim.ClaimType);
                            command.Parameters.AddWithValue("@ClaimValue", claim.ClaimValue);
                            command.Parameters.AddWithValue("@UserId", user.Id);
                            command.Parameters.AddWithValue("@ClaimID", claim.Id);

                            command.Parameters.AddWithValue("@IsActive", "True");
                            command.Parameters.AddWithValue("@DisplayLabel", claim.Notes);
                            command.Parameters.AddWithValue("@Discriminator", "ApplicationUserClaim");
                            command.Parameters.AddWithValue("@User_Id", user.Id);

                            int _result = 0;
                            try
                            {
                                _result = command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {

                                throw;
                            }
                            // Check Error
                            if (_result < 0)
                                Console.WriteLine("Error inserting data into Database!");
                        }
                    }
                    connection.Close();
                }
            }


            if (!context.Manufacturers.Any(m => m.Name == "Shivam"))
            {
                var manufactures = new List<Manufacturer>
                {
                    new Manufacturer { Code = "#111" , Name= "Shivam" ,SupplierID = 0 , CreatedDate= DateTime.Now, UpdatedDate = DateTime.Now,
                        Sort = 1 , Description = "Dummy Manufacturer",Notes = "Dummy Manufacturer"
                    }
                };
                manufactures.ForEach(m => context.Manufacturers.Add(m));
                context.SaveChanges();
            }

            if (!context.UnitOfMeasures.Any(u => u.UnitOfMeasuresName == "Unit" || u.UnitOfMeasuresName == "Kg" || u.UnitOfMeasuresName == "Litre"))
            {
                var UOMs = new List<UnitOfMeasures>
                {
                    new UnitOfMeasures {UnitOfMeasuresName = "Unit"},
                    new UnitOfMeasures {UnitOfMeasuresName = "Kg"},
                    new UnitOfMeasures {UnitOfMeasuresName = "Each"},
                    new UnitOfMeasures {UnitOfMeasuresName = "Litre"},
                };

                UOMs.ForEach(u => context.UnitOfMeasures.Add(u));
                context.SaveChanges();
            }


            if (!context.ProductStatus.Any(p => p.Name == "New"))
            {
                var ProductStatus = new List<ProductStatus>
                {
                    new ProductStatus{Name="New",IsActive = true, CreatedDate= DateTime.Now, UpdatedDate = DateTime.Now,Sort=0,Description="",Notes=""}
                   
                };
                ProductStatus.ForEach(p => context.ProductStatus.Add(p));
                context.SaveChanges();
            }


            //if (!context.Cateogries.Any(c => c.CategoryName == "Men"))
            //{
            //    var categories = new List<Category>
            //    {
            //        new Category { CategoryName = "Men", IsActive = true, ParentCategory = 0 ,CategoryImage = "", CreatedDate = DateTime.Now,
            //        UpdatedDate = DateTime.Now, Sort = 1 ,Description = "Parent Men Category" , Notes = "Parent Men Category" 
            //        }
            //    };
            //    categories.ForEach(c => context.Cateogries.Add(c));
            //    context.SaveChanges();                
            //}


            if (!context.ProductAttributes.Any(c => c.AttributeName == "Size"))
            {
                var ProductAttributes = new List<ProductAttributes>
                {
                    new ProductAttributes { AttributeName = "Size",AttributeDescription="Size of product"},
                    new ProductAttributes { AttributeName = "Color",AttributeDescription="Color of product"},
                    new ProductAttributes { AttributeName = "Brand",AttributeDescription="Brand of product"},
                };
                ProductAttributes.ForEach(c => context.ProductAttributes.Add(c));
                context.SaveChanges();
            }



            



            //if (!context.OrderStatuses.Any(p => p.Status == "Confirmed" || p.Status == "Cancelled" || p.Status == "On_Hold" || p.Status == "Delivered"))
            //{
            //    var OrderStatus = new List<OrderStatus>
            //    {
            //        new OrderStatus{Status="Confirmed"},
            //        new OrderStatus{Status="Cancelled"},
            //        new OrderStatus{Status="On_Hold"},
            //        new OrderStatus{Status="Delivered"}

            //    };
            //    OrderStatus.ForEach(p => context.OrderStatuses.Add(p));
            //    context.SaveChanges();
            //}


            //if (!context.Users.Any(u => u.UserName == "Test"))
            //{
            //    // var passwordHash = new PasswordHasher();
            //    // string password = passwordHash.HashPassword("Shivam@123");

            //    var store = new UserStore<ApplicationUser>(context);
            //    var manager = new UserManager<ApplicationUser>(store);
            //    var user = new ApplicationUser { UserName = "Test" };

            //    manager.Create(user, "Shivam@123");
            //    manager.AddToRole(user.Id, "superadmin");
            //}

           

           


            //if (!context.Plans.Any(p => p.PlanName == "PlatinumPackage" || p.PlanName == "GoldPackage" || p.PlanName == "SilverPackage"))
            //{
            //    var Plans = new List<Plans>
            //    {
            //        new Plans {PlanName = "PlatinumPackage", Plancode = "#111",CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 1 ,Description="Dummy plan created for demonstration purposes",Notes="Dummy plan created for demonstration purposes" },
            //        new Plans {PlanName = "GoldPackage", Plancode = "#112",CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 2 ,Description="Dummy plan created for demonstration purposes",Notes="Dummy plan created for demonstration purposes" },
            //        new Plans {PlanName = "SilverPackage", Plancode = "#113",CreatedDate = DateTime.Now,UpdatedDate = DateTime.Now,Sort = 3 ,Description="Dummy plan created for demonstration purposes",Notes="Dummy plan created for demonstration purposes" },
            //    };

            //    Plans.ForEach(p => context.Plans.Add(p));
            //    context.SaveChanges();
            //}


            //if (!context.Suppliers.Any(s => s.FirstName == "Thakur"))
            //{

            //    var userid = context.Users.FirstOrDefault(u => u.UserName == "Test").Id;

            //    try
            //    {

            //        var Suppliers = new List<Supplier>
            //    {
            //        new Supplier{CompanyName= "Shivam",FirstName="Thakur", LastName="Sindhi",Title="Sindhi",Address1="Bus Stand Nathdwara",Address2="Bus Stand Nathdwara",City="Nathdwara",State="Rajasthan",PostalCode="313301",Country="India",
            //        Phone = "123456789", Email="test@test.com",URL="test URl",Logo="Test",SupplierType="Super Supplier",ParentSupplierID=1,
            //        RegisteredByID = "1",UserID= userid,PlanID=1,CreatedDate=DateTime.Now,UpdatedDate= DateTime.Now, Sort=1,
            //        Description = "This is a test supplier added for demo purpose",
            //        Notes = "This is a test supplier added for demo purpose", 
            //        }
            //    };

            //        Suppliers.ForEach(s => context.Suppliers.Add(s));
            //        context.SaveChanges();
            //    }
            //    catch (DbEntityValidationException e)
            //    {
            //        foreach (var eve in e.EntityValidationErrors)
            //        {
            //            System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //                eve.Entry.Entity.GetType().Name, eve.Entry.State);



            //            foreach (var ve in eve.ValidationErrors)
            //            {
            //                System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
            //                    ve.PropertyName, ve.ErrorMessage);

            //                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
            //                    ve.PropertyName, ve.ErrorMessage);
            //            }
            //        }
            //        throw;

            //    }

           

            

            //if (!context.Products.Any(p => p.ProductName == "Product1"))
            //{
            //    var Products = new List<Product>
            //    {
            //        new Product { 
            //            ProductName = "Product1" , ProductCode= "#111" , Ranking = "Top" , SKU = "", IDSKU ="",
            //            SupplierID = 1 , ManuFacturerID = 1 , UnitOfMeasuresId = 1, CateogryID = 1,
            //            CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now, Sort =1, 
            //            Description = "Dummy Product", Notes = "Dummy Product"
            //        },
            //        new Product { 
            //            ProductName = "Product2" , ProductCode= "#112" , Ranking = "Top" , SKU = "", IDSKU ="",
            //            SupplierID = 1 , ManuFacturerID = 1 , UnitOfMeasuresId = 1, CateogryID = 1,
            //            CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now, Sort =1, 
            //            Description = "Dummy Product", Notes = "Dummy Product"
            //        },
            //        new Product { 
            //            ProductName = "Product3" , ProductCode= "#113" , Ranking = "Top" , SKU = "", IDSKU ="",
            //            SupplierID = 1 , ManuFacturerID = 1 , UnitOfMeasuresId = 1, CateogryID = 1,
            //            CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now, Sort =1, 
            //            Description = "Dummy Product", Notes = "Dummy Product"
            //        }
            //        ,new Product { 
            //            ProductName = "Product4" , ProductCode= "#114" , Ranking = "Top" , SKU = "", IDSKU ="",
            //            SupplierID = 1 , ManuFacturerID = 1 , UnitOfMeasuresId = 1, CateogryID = 1,
            //            CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now, Sort =1, 
            //            Description = "Dummy Product", Notes = "Dummy Product"
            //        }
            //        ,new Product { 
            //            ProductName = "Product5" , ProductCode= "#115" , Ranking = "Top" , SKU = "", IDSKU ="",
            //            SupplierID = 1 , ManuFacturerID = 1 , UnitOfMeasuresId = 1, CateogryID = 1,
            //            CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now, Sort =1, 
            //            Description = "Dummy Product", Notes = "Dummy Product"
            //        }
            //        ,new Product { 
            //            ProductName = "Product6" , ProductCode= "#116" , Ranking = "Top" , SKU = "", IDSKU ="",
            //            SupplierID = 1 , ManuFacturerID = 1 , UnitOfMeasuresId = 1, CateogryID = 1,
            //            CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now, Sort =1, 
            //            Description = "Dummy Product", Notes = "Dummy Product"
            //        }
            //        ,new Product { 
            //            ProductName = "Product7" , ProductCode= "#117" , Ranking = "Top" , SKU = "", IDSKU ="",
            //            SupplierID = 1 , ManuFacturerID = 1 , UnitOfMeasuresId = 1, CateogryID = 1,
            //            CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now, Sort =1, 
            //            Description = "Dummy Product", Notes = "Dummy Product"
            //        }
            //        ,new Product { 
            //            ProductName = "Product8" , ProductCode= "#118" , Ranking = "Top" , SKU = "", IDSKU ="",
            //            SupplierID = 1 , ManuFacturerID = 1 , UnitOfMeasuresId = 1, CateogryID = 1,
            //            CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now, Sort =1, 
            //            Description = "Dummy Product", Notes = "Dummy Product"
            //        }
            //        ,new Product { 
            //            ProductName = "Product9" , ProductCode= "#119" , Ranking = "Top" , SKU = "", IDSKU ="",
            //            SupplierID = 1 , ManuFacturerID = 1 , UnitOfMeasuresId = 1, CateogryID = 1,
            //            CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now, Sort =1, 
            //            Description = "Dummy Product", Notes = "Dummy Product"
            //        }
            //        ,new Product { 
            //            ProductName = "Product10" , ProductCode= "#1110" , Ranking = "Top" , SKU = "", IDSKU ="",
            //            SupplierID = 1 , ManuFacturerID = 1 , UnitOfMeasuresId = 1, CateogryID = 1,
            //            CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now, Sort =1, 
            //            Description = "Dummy Product", Notes = "Dummy Product"
            //        }
            //    };
            //    Products.ForEach(p => context.Products.Add(p));
            //    context.SaveChanges();
            //}

        }
    }
}

