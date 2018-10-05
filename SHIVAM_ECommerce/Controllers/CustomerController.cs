using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SHIVAM_ECommerce.Models;
using Microsoft.AspNet.Identity;
using SHIVAM_ECommerce.Attributes;
using SHIVAM_ECommerce.Repository;
using System.Linq.Dynamic;
using SHIVAM_ECommerce.Extensions;
using System.Configuration;
namespace SHIVAM_ECommerce.Controllers
{
    [CustomAuthorize]
    public class CustomerController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IRepository<Customer> _repository = null;


        public CustomerController()
        {
            this._repository = new Repository<Customer>();
        }

        public ActionResult LoadData()
        {

            try
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
                var _orderIds = CurrentUserData.SupplierID != -1 ? db.OrderItems.Where(x => x.SupplierID == CurrentUserData.SupplierID).Select(x => x.Orders_Id) : null;
                var _customerIds = CurrentUserData.SupplierID != -1 && _orderIds != null ? db.Orders.Include(o => o.customer).Where(x => _orderIds.Contains(x.Id)).Select(x => x.CustomerID) : null;

                var customers = CurrentUserData.SupplierID == -1 ? db.Customers : db.Customers.Where(x => _customerIds.Contains(x.Id));

                // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
                var v = customers.Select(x => x);
                if (!string.IsNullOrEmpty(searchitem))
                {

                    v = v.Where(b => b.FirstName.Contains(searchitem) || b.LastName.Contains(searchitem) || b.Phone.Contains(searchitem) || b.Email.Contains(searchitem));
                }
                sortColumn = sortColumn == "UserName" ? "FirstName" : sortColumn;
                sortColumn = sortColumn == "FullName" ? "FirstName" : sortColumn;

                //SORT
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                recordsTotal = v.Count();
                var data = v.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, UserName = x.User != null ? x.User.UserName : "", FullName = x.FirstName + " " + x.LastName, x.Phone, x.Email }) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }




        public ActionResult LoadCustomerOrders()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchitem = Request["search[value]"];
            //Find Order Column
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var CustomerID = !string.IsNullOrEmpty(Request["CustomerID"]) ? Convert.ToInt16(Request["CustomerID"]) : CurrentUserData.SupplierID;

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var orders = db.Orders.Include(o => o.customer).Where(x => x.CustomerID == CustomerID);
            var v = (from a in orders select a);
            if (!string.IsNullOrEmpty(searchitem))
            {

                v = v.Where(b => b.customer.FirstName.Contains(searchitem) || b.status.Status.Contains(searchitem) || b.OrderNumber.Contains(searchitem));
            }
            v = v.OrderBy(x => x.OrderDate);
            //SORT
            sortColumn = sortColumn == "Paid" ? "IsPaid" : sortColumn;

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.OrderNumber, Name = x.customer.FirstName, x.OrderDate, x.ShipDate, Status = x.status.Status, Paid = x.IsPaid == true ? "Paid" : "UnPaid", x.TotalPrice, }) }, JsonRequestBehavior.AllowGet);
        }


        // GET: /Customer/
        public async Task<ActionResult> Index()
        {
            var customers = db.Customers.Include(c => c.User);
            return View(await customers.ToListAsync());
        }

        // GET: /Customer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: /Customer/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "Id", "UserName");
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        // POST: /Customer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FirstName,LastName,Phone,Email,CreditCard,CreditCardType,CardExpMo,CardExpYr,UserID,CreatedDate,UpdatedDate,Sort,Description,Notes,UserName,Password,ConfirmPassword")] Customer customer)
        {
            var _controller = new AccountController();

            if (ModelState.IsValid)
            {
                customer.CreatedDate = DateTime.Now;
                customer.UpdatedDate = DateTime.Now;
                customer.Sort = 33;


                db.Customers.Add(customer);
                var user = new ApplicationUser() { UserName = customer.UserName, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };
                IdentityResult result = await _controller.UserManager.CreateAsync(user, customer.Password);
                if (result.Succeeded)
                {
                    customer.UserID = user.Id;
                    db.SaveChanges();
                    _controller.UserManager.AddToRole(user.Id, "Customer");
                    string email = customer.Email;
                    string username = customer.UserName;
                    string adminemail = ConfigurationManager.AppSettings["adminemail"].ToString();
                    string admintemple = ConfigurationManager.AppSettings["admintempleforcustomer"].ToString();
                    string subject = ConfigurationManager.AppSettings["subject"].ToString();
                    string AdminUserName = ConfigurationManager.AppSettings["adminusername"].ToString();
                    var sendemail = new EmailService.Service.EmailService();
                    sendemail.SendEmail(email, "addcustomer.html", "verfication", username,null,null);
                    sendemail.SendEmail(adminemail, admintemple, subject, AdminUserName,null,null);

                    this.AddNotification("Customer Created successfully.", NotificationType.SUCCESS);
                    #region Saveinemailrecord
                    var emailrecord = new emailrecord();
                    emailrecord.Email_Sender = "yogesh.d@shivamitconsultancy.com";
                    emailrecord.Email_Receiver = email;
                    emailrecord.Send_Date = DateTime.Now.ToString();
                    emailrecord.Subject = subject;
                    emailrecord.Message = "New Customer Added ";
                    db.EmailRecord.Add(emailrecord);
                    db.SaveChanges();


                    #endregion

                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrors(result);
                }

            }
            return View(customer);
        }

        // GET: /Customer/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "Id", "UserName", customer.UserID);
            return View(customer);
        }

        // POST: /Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FirstName,LastName,Phone,Email,CreditCard,CreditCardType,CardExpMo,CardExpYr,UserID,CreatedDate,UpdatedDate,Sort,Description,Notes,UserName,Password,ConfirmPassword")] Customer customer)
        {

            customer.CreatedDate = DateTime.Now;
            customer.UpdatedDate = DateTime.Now;
            customer.Sort = 33;

            customer.UserName = "UserName";
            customer.Password = "Password";
            customer.ConfirmPassword = "Password";

            //if (ModelState.IsValid)
            //{
            db.Entry(customer).State = EntityState.Modified;
            await db.SaveChangesAsync();
            this.AddNotification("Customer updated successfully.", NotificationType.SUCCESS);
            return RedirectToAction("Index");
            //}
            ViewBag.UserID = new SelectList(db.Users, "Id", "UserName", customer.UserID);
            return View(customer);
        }

        // GET: /Customer/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: /Customer/Delete/5
        [HttpGet, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Customer customer = await db.Customers.FindAsync(id);
            db.Customers.Remove(customer);
            await db.SaveChangesAsync();
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
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
