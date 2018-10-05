using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SHIVAM_ECommerce.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using ExportImplementation;
using System.Diagnostics;
using System.IO;
using SHIVAM_ECommerce.Repository;
using System.Configuration;
using SHIVAM_ECommerce.Extensions;
using SHIVAM_ECommerce.Attributes;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using SHIVAM_ECommerce.ViewModels;
using System.Data.Entity.Validation;
namespace SHIVAM_ECommerce.Controllers
{

    // [Authorize(Roles = "superadmin,Admin")]
    public class SupplierController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        private IRepository<Supplier> _repository = null;
        public SupplierController()
        {
            this._repository = new Repository<Supplier>();
        }


        [CustomAuthorize()]
        // GET: /Supplier/
        public ActionResult Index()
        {
            return View();
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
            var v = (from a in _repository.GetAll() select a);
            if (!string.IsNullOrEmpty(searchitem))
            {

                v = v.Where(b => b.CompanyName != null && b.CompanyName.ToLower().Contains(searchitem.ToLower()) || b.FirstName != null && b.FirstName.ToLower().Contains(searchitem.ToLower()) || b.Phone != null && b.Phone.ToLower().Contains(searchitem.ToLower()) || b.City != null && b.City.ToLower().Contains(searchitem.ToLower()));
            }
            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.CompanyName, x.FirstName, x.LastName, x.Logo, x.Phone, x.City }) }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AllSupplier()
        {
           
            var suppliers = db.Suppliers.Include(s => s.Plan).Include(s => s.User).Select(p => new { Name = p.FirstName + p.LastName, Id = p.Id }); ;
            return Json(suppliers.ToList(), JsonRequestBehavior.AllowGet);

        }

        // GET: /Supplier/Details/5
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

        // GET: /Supplier/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            //var allplans = db.Plans.Include(x => x.Features).ToList();
            //ViewBag.allplans = allplans;
            //return View(new Supplier());
            return RedirectToAction("CreateNew");
        }


        public ActionResult CreateNew()
        {
            var allplans = new List<Plans>();
            try
            {
                 allplans = db.Plans.Where(x => x.IsActive == true).Include(x => x.Features).ToList();
            }
            catch (Exception ex)
            {                
                throw;
            }          
            ViewBag.allplans = allplans;
            return View(new SupplierVM());
        }
        [HttpPost]
        public async Task<ActionResult> CreateNew([Bind(Include = "Id,Name,LastName,Email,UserName,Password,PlanID")] SupplierVM supplier)
        {

            try
            {


                if (ModelState.IsValid)
                {
                    var _controller = new AccountController();
                    var _plan = db.Plans.Where(x => x.Id == supplier.PlanID).FirstOrDefault();
                    var _supplier = new Supplier();
                    _supplier.CreatedDate = DateTime.Now;
                    _supplier.UpdatedDate = DateTime.Now;
                    _supplier.Sort = 33;
                    _supplier.ParentSupplierID = 0;
                    _supplier.ProductCount = _plan.ProductBucketCount;
                    _supplier.PlanStartDate = DateTime.Now;
                    _supplier.PlanEndDate = DateTime.Now.AddDays(_plan.PlanFrequency == "1" ? 30 : 365);
                    _supplier.UserCount = _plan.UserBucketCount;
                    _supplier.FirstName = supplier.Name;
                    _supplier.Email = supplier.Email;
                    _supplier.UserName = supplier.UserName;
                    _supplier.LastName = supplier.LastName;
                    _supplier.PlanID = supplier.PlanID;
                    _supplier.Password = supplier.Password;
                    db.Suppliers.Add(_supplier);
                    var user = new ApplicationUser() { UserName = supplier.UserName, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };
                    IdentityResult result = await _controller.UserManager.CreateAsync(user, supplier.Password);
                    if (result.Succeeded)
                    {
                        _supplier.UserID = user.Id;
                        db.SaveChanges();
                        _controller.UserManager.AddToRole(user.Id, "Supplier");


                        //Add Claims to the AspNetUserClaims table for the supplier registerd.
                        var Claims = db.Claims.Where(x => x.Role == "Supplier" && x.IsActive==true).ToList();
                        var _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString();
                        using (SqlConnection connection = new SqlConnection(_connectionString))
                        {
                            connection.Open();

                            foreach (var claim in Claims)
                            {
                                String query = "INSERT INTO [dbo].[AspNetUserClaims]([ClaimType],[ClaimValue],[ClaimID],[IsActive],[DisplayLabel],[Discriminator],[User_Id]) VALUES(@ClaimType,@ClaimValue,@ClaimID,@IsActive,@DisplayLabel,@Discriminator,@User_Id)";
                                using (SqlCommand command = new SqlCommand(query, connection))
                                {
                                    command.Parameters.AddWithValue("@ClaimType", claim.ClaimType);
                                    command.Parameters.AddWithValue("@ClaimValue", claim.ClaimValue);
                                    //command.Parameters.AddWithValue("@UserId", user.Id);
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

                        //foreach (var claim in Claims)



                        //Send confirmation mail to user and admin
                        string email = supplier.Email;
                        string username = _supplier.UserName;
                        string adminemail = ConfigurationManager.AppSettings["adminemail"].ToString();
                        string admintemple = ConfigurationManager.AppSettings["admintemple"].ToString();
                        string Supplierregisters = ConfigurationManager.AppSettings["Supplierregister"].ToString();
               
                        string subject = ConfigurationManager.AppSettings["subject"].ToString();
                        string AdminUserName = ConfigurationManager.AppSettings["adminusername"].ToString();
                        var sendemail = new EmailService.Service.EmailService();
                        sendemail.SendEmail(email, Supplierregisters, "Successfully Created", username, supplier.UserName, supplier.Password);
                        sendemail.SendEmail(adminemail, admintemple, subject, AdminUserName,null, null);
                        this.AddNotification("Created successfully.", NotificationType.SUCCESS);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrors(result);
                    }


                }
                ViewBag.PlanID = new SelectList(db.Plans, "Id", "PlanName", supplier.PlanID);
                var allplans = db.Plans.Where(x => x.IsActive == true).Include(x => x.Features).ToList();
                ViewBag.allplans = allplans;
                return View(supplier);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
                ViewBag.PlanID = new SelectList(db.Plans, "Id", "PlanName", supplier.PlanID);
                var allplans = db.Plans.Where(x => x.IsActive == true).Include(x => x.Features).ToList();
                ViewBag.allplans = allplans;
                return View(supplier);
            }
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "Id,CompanyName,FirstName,LastName,Title,Address1,Address2,City,State,PostalCode,Country,Phone,Email,URL,Logo,SupplierType,UserID,PlanID,UserName,Password")] Supplier supplier, HttpPostedFileBase file)
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
                var _plan = db.Plans.Where(x => x.Id == supplier.PlanID).FirstOrDefault();

                supplier.CreatedDate = DateTime.Now;
                supplier.UpdatedDate = DateTime.Now;
                supplier.Sort = 33;
                supplier.ParentSupplierID = 0;
                supplier.ProductCount = _plan.ProductBucketCount;
                supplier.PlanStartDate = DateTime.Now;
                supplier.PlanEndDate = DateTime.Now.AddDays(_plan.PlanFrequency == "1" ? 30 : 365);
                supplier.UserCount = _plan.UserBucketCount;

                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/SupplierImage"), pic);
                    // file is uploaded
                    file.SaveAs(path);

                    supplier.Logo = pic;

                    // save the image path path to the database or you can send image 
                    // directly to database
                    // in-case if you want to store byte[] ie. for DB
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }



                db.Suppliers.Add(supplier);
                var user = new ApplicationUser() { UserName = supplier.UserName, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };
                IdentityResult result = await _controller.UserManager.CreateAsync(user, supplier.Password);
                if (result.Succeeded)
                {
                    supplier.UserID = user.Id;
                    db.SaveChanges();
                    _controller.UserManager.AddToRole(user.Id, "Supplier");


                    //Add Claims to the AspNetUserClaims table for the supplier registerd.
                    var Claims = db.Claims.Where(x => x.Role == "Supplier").ToList();
                    var _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString();
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        foreach (var claim in Claims)
                        {
                            String query = "INSERT INTO [dbo].[AspNetUserClaims]([ClaimType],[ClaimValue],[ClaimID],[IsActive],[DisplayLabel],[Discriminator],[User_Id]) VALUES(@ClaimType,@ClaimValue,@ClaimID,@IsActive,@DisplayLabel,@Discriminator,@User_Id)";
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@ClaimType", claim.ClaimType);
                                command.Parameters.AddWithValue("@ClaimValue", claim.ClaimValue);
                                //command.Parameters.AddWithValue("@UserId", user.Id);
                                command.Parameters.AddWithValue("@ClaimID", claim.Id);
                                command.Parameters.AddWithValue("@IsActive", "True");
                                command.Parameters.AddWithValue("@DisplayLabel", "abc");
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

                    //foreach (var claim in Claims)
                    //{
                    //    ApplicationUserClaim UserClaim = new ApplicationUserClaim();
                    //    //_controller.UserManager.AddClaim(user.Id, new Claim(claim.ClaimType, claim.ClaimValue)); 
                    //    UserClaim.ClaimType = claim.ClaimType;
                    //    UserClaim.ClaimValue = claim.ClaimValue;
                    //    UserClaim.User = db.Users.FirstOrDefault(x => x.Id == user.Id);

                    //    UserClaim.IsActive = true;
                    //    UserClaim.User_Id = user.Id;

                    //    db.AspNetUserClaims.Add(UserClaim);
                    //}
                    //db.SaveChanges();


                    //Send confirmation mail to user and admin
                    string email = supplier.Email;
                    string username = supplier.UserName;
                    string adminemail = ConfigurationManager.AppSettings["adminemail"].ToString();
                    string admintemple = ConfigurationManager.AppSettings["admintemple"].ToString();
                    string Supplierregisters = ConfigurationManager.AppSettings["Supplierregister"].ToString();
                    string subject = ConfigurationManager.AppSettings["subject"].ToString();
                    string AdminUserName = ConfigurationManager.AppSettings["adminusername"].ToString();
                    var sendemail = new EmailService.Service.EmailService();
                    sendemail.SendEmail(email, Supplierregisters, "Successfully Created", username, supplier.UserName, supplier.Password);
                    sendemail.SendEmail(adminemail, admintemple, subject, AdminUserName, null, null);
                    this.AddNotification("Created successfully.", NotificationType.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrors(result);
                }

            }


            ViewBag.PlanID = new SelectList(db.Plans, "Id", "PlanName", supplier.PlanID);
            var allplans = db.Plans.ToList();
            ViewBag.allplans = allplans;
            return View(supplier);

        }


        [HttpPost]
        public ActionResult UniqueEmail(string Email)
        {
            try
            {

                var _user = db.Suppliers.Where(x => x.Email == Email).FirstOrDefault();
                if (_user != null)
                {
                    return Json(new { Success = true, ex = "", IsAlreadyExist = true });
                }
                return Json(new { Success = true, ex = "", IsAlreadyExist = false });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ex = ex.Message.ToString(), IsAlreadyExist = false });
            }
        }


        [HttpPost]
        public ActionResult UniqueUserName(string UserName)
        {
            try
            {
                var _controller = new AccountController();
                var _user = _controller.UserManager.FindByName(UserName);
                if (_user != null)
                {
                    return Json(new { Success = true, ex = "", IsAlreadyExist = true });
                }
                return Json(new { Success = true, ex = "", IsAlreadyExist = false });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, ex = ex.Message.ToString(), IsAlreadyExist = false });
            }
        }
        // GET: /Supplier/Edit/5
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
            // ViewBag.UserID = new SelectList(db.IdentityUsers, "Id", "UserName", supplier.UserID);
            return View(supplier);
        }


        public FileContentResult DownloadAsExcel()
        {
            var listWithPerson = db.Suppliers.Include(s => s.Plan).Include(s => s.User).ToList();
            var export = new ExportExcel2007<Supplier>();
            var data = export.ExportResult(listWithPerson);
            var _path = Server.MapPath("~/DownloadedFiles/a.xlsx");
            System.IO.File.WriteAllBytes(_path, data);
            Process.Start(_path);

            return null;
        }

        // POST: /Supplier/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CompanyName,FirstName,LastName,Title,Address1,Address2,City,State,PostalCode,Country,Phone,Email,URL,Logo,SupplierType,UserID,PlanID,UserName,Password,PlanEndDate,PlanStartDate,ProductCount,CreatedDate,UserCount")] Supplier supplier, HttpPostedFileBase file)
        {


            if (ModelState.IsValid)
            {

                supplier.UpdatedDate = DateTime.Now;


                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/SupplierImage"), pic);
                    // file is uploaded
                    file.SaveAs(path);

                    supplier.Logo = pic;

                    // save the image path path to the database or you can send image 
                    // directly to database
                    // in-case if you want to store byte[] ie. for DB
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }

                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                this.AddNotification("Updated successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.PlanID = new SelectList(db.Plans, "Id", "PlanName", supplier.PlanID);
            return View(supplier);
        }

        // GET: /Supplier/Delete/5
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

        // POST: /Supplier/Delete/5

        [HttpPost]
        public ActionResult Delete(int id)
        {

            try
            {
                var _products = db.Products.Where(x => x.SupplierID == id).Count();
                var _fsupplier = db.FeaturedSuppliers.Where(x => x.SupplierID == id).Count();
                if (_products == 0 && _fsupplier == 0)
                {

                    Supplier supplier = db.Suppliers.Find(id);
                    db.Suppliers.Remove(supplier);
                    db.SaveChanges();
                    return Json(new { Success = true, ex = "" });
                }
                else
                {
                    return Json(new { Success = false, ex = "This supplier is Associated with some product or featured supplier, unable to delete this." });
                }
            }
            catch (Exception ex)
            {

                return Json(new { Success = false, ex = ex.Message.ToString() });
            }

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
