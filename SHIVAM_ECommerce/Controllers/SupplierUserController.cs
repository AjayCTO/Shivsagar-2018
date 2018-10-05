using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SHIVAM_ECommerce.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using SHIVAM_ECommerce.Attributes;
using SHIVAM_ECommerce.Repository;
using System.Linq.Dynamic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using SHIVAM_ECommerce.Extensions;
using System.Data.SqlClient;
using System.Configuration;
namespace SHIVAM_ECommerce.Controllers
{
    //[CustomAuthorize(Roles="superadmin,Admin,Supplier")]
    public class SupplierUserController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        private IRepository<Supplier> _repository = null;
        public SupplierUserController()
        {
            this._repository = new Repository<Supplier>();
        }

        // GET: /SupplierUser/
        public ActionResult Index()
        {
            var suppliers = db.Suppliers.Include(s => s.Plan).Include(s => s.User).Where(x => x.ParentSupplierID == CurrentUserData.SupplierID);
            return View(suppliers.ToList());
        }




        public ActionResult LoadData()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchitem = Request["search[value]"];
            //Find Order Column
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
            //var v = (from a in _repository.GetAll() select a);
            var v = db.Suppliers.Include(s => s.Plan).Include(s => s.User).Where(x => x.ParentSupplierID == CurrentUserData.SupplierID);
            if (!string.IsNullOrEmpty(searchitem))
            {

                v = v.Where(b => b.CompanyName.Contains(searchitem));
            }
            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.CompanyName, x.FirstName, x.LastName, x.Logo, x.Phone, x.City, Address = x.Address1 + " " + x.Address2 }) }, JsonRequestBehavior.AllowGet);
        }












        // GET: /SupplierUser/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: /SupplierUser/Create
        public ActionResult Create()
        {
            var allplans = db.Plans.ToList();
            ViewBag.allplans = allplans;
            ViewBag.PlanID = new SelectList(db.Plans, "Id", "PlanName");
            //  ViewBag.UserID = new SelectList(db.IdentityUsers, "Id", "UserName");
            return View();
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        // POST: /SupplierUser/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CompanyName,FirstName,LastName,Title,Address1,Address2,City,State,PostalCode,Country,Phone,Email,URL,Logo,SupplierType,ParentSupplierID,RegisteredByID,UserID,PlanID,UserName,Password")] Supplier supplier)
        {
            var _controller = new AccountController();

            if (!string.IsNullOrEmpty(supplier.UserName))
            {

                var _user = _controller.UserManager.FindByNameAsync(supplier.UserName);
                if (_user.Result != null)
                {
                    ModelState.AddModelError("Already Exist", "User already exist please provide different user name");
                }
            }
            if (ModelState.IsValid)
            {
                supplier.CreatedDate = (DateTime)DateTime.Now;
                supplier.UpdatedDate = (DateTime)DateTime.Now;
                supplier.Sort = 33;
                supplier.ParentSupplierID = CurrentUserData.SupplierID;
                supplier.RegisteredByID = CurrentUserData.UserID;
                supplier.PlanStartDate = (DateTime)DateTime.Now;
                supplier.PlanEndDate = CurrentUserData.PlanEndDate;
                supplier.PlanID = CurrentUserData.PlanId;
                supplier.CompanyName = CurrentUserData.CompanyName;
                db.Suppliers.Add(supplier);
                var user = new ApplicationUser() { UserName = supplier.UserName, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };
                IdentityResult result = await _controller.UserManager.CreateAsync(user, supplier.Password);
                if (result.Succeeded)
                {
                    supplier.UserID = user.Id;
                    db.SaveChanges();
                    _controller.UserManager.AddToRole(user.Id, "SupplierUser");

                    //Add Claims to the AspNetUserClaims table for the supplier registerd.
                    var Claims = db.Claims.Where(x => x.Description == "SupplierUser").ToList();

                    var listOfUserClaims = new List<IdentityUserClaim>();
                    var _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString();


                    // _controller.UserManager.AddClaim(user.Id, new Claim(claim.ClaimType, claim.ClaimValue));
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        foreach (var claim in Claims)
                        {
                            String query = "INSERT INTO [dbo].[AspNetUserClaims]([ClaimType],[ClaimValue],[UserId],[ClaimID],[IsActive],[DisplayLabel],[Discriminator],[User_Id]) VALUES(@ClaimType,@ClaimValue,@UserId,@ClaimID,@IsActive,@DisplayLabel,@Discriminator,@User_Id)";
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

                                int _result = command.ExecuteNonQuery();

                                // Check Error
                                if (_result < 0)
                                    Console.WriteLine("Error inserting data into Database!");
                            }
                        }
                        connection.Close();

                    }
                    db.SaveChanges();
                    this.AddNotification("User Created successfully.", NotificationType.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrors(result);
                }

            }

            ViewBag.PlanID = new SelectList(db.Plans, "Id", "PlanName", supplier.PlanID);
            return View(supplier);
        }

        // GET: /SupplierUser/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlanID = new SelectList(db.Plans, "Id", "PlanName", supplier.PlanID);
            return View(supplier);
        }

        // POST: /SupplierUser/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CompanyName,FirstName,LastName,Title,Address1,Address2,City,State,PostalCode,Country,Phone,Email,URL,Logo,SupplierType,ParentSupplierID,RegisteredByID,UserID,PlanID,CreatedDate,UpdatedDate,Sort,Description,Notes")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlanID = new SelectList(db.Plans, "Id", "PlanName", supplier.PlanID);
            return View(supplier);
        }

        // GET: /SupplierUser/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: /SupplierUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Supplier supplier = db.Suppliers.Find(id);
            db.Suppliers.Remove(supplier);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
