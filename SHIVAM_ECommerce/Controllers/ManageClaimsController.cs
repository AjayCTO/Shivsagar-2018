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
using SHIVAM_ECommerce.Extensions;
using SHIVAM_ECommerce.Repository;
using System.Security.Claims;
using System.Linq.Dynamic;
using SHIVAM_ECommerce.Attributes;

namespace SHIVAM_ECommerce.Controllers
{
    [CustomAuthorize]
    public class ManageClaimsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private IRepository<Claims> _repository = null;
        public ManageClaimsController()
        {
            this._repository = new Repository<Claims>();
        }



        // GET: /UserClaims/
        public async Task<ActionResult> Index()
        {

            if (CurrentUserData.IsSuperAdmin)
            {
                ViewBag.UserName = db.Users.ToList();
            }
            else
            {

                var supplierUsers = db.Suppliers.Where(x => x.ParentSupplierID == CurrentUserData.SupplierID);

                List<ApplicationUser> UserNames = new List<ApplicationUser>();

                foreach (var SUser in supplierUsers)
                {
                    var UserName = db.Users.FirstOrDefault(x => x.Id == SUser.UserID);
                    UserNames.Add(UserName);

                }
                ViewBag.UserName = UserNames;
            }

            return View(await db.Claims.ToListAsync());
        }



        public ActionResult LoadData()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchitem = Request["search[value]"];
            var UserName = Request["UserName"];
            var UserID = "";
            //Get UserID from the aspnetuser table using UserName

            var User = db.Users.FirstOrDefault(x => x.UserName.ToLower() == UserName.ToLower());

            if (User != null)
            {
                UserID = User.Id;
            }

            //Find Order Column
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
            var v = db.AspNetUserClaims.AsEnumerable();

            if (!string.IsNullOrEmpty(UserID))
            {

                v = v.Where(x => x.User.Id == UserID).ToList();
            }
            if (!string.IsNullOrEmpty(searchitem))
            {

                v = v.Where(b => b.ClaimType.Contains(searchitem)).ToList();
            }
            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.ClaimType, x.ClaimValue, x.IsActive, x.DisplayLabel }) }, JsonRequestBehavior.AllowGet);
        }





        // GET: /UserClaims/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Claims claims = await db.Claims.FindAsync(id);
            if (claims == null)
            {
                return HttpNotFound();
            }
            return View(claims);
        }

        // GET: /UserClaims/Create
        public ActionResult Create()
        {
            ViewBag.Roles = db.Roles;
            return View();
        }

        // POST: /UserClaims/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ClaimType,ClaimValue,IsActive,CreatedDate,UpdatedDate,Sort,Title,ClaimGroup,Description,Notes,Role")] Claims claims)
        {
            if (ModelState.IsValid)
            {
                Claims claim = new Claims();
                claim.ClaimType=claims.ClaimType;
                claim.ClaimValue=claims.ClaimValue;
                claim.IsActive=claims.IsActive;
                claim.CreatedDate=DateTime.Now;
                claim.UpdatedDate = DateTime.Now;
                claim.Role = claims.Role;
                claim.Notes = claims.Notes;
                claim.Title = claims.Title;
                claim.Description = claims.Description;
                claim.ClaimGroup = claims.ClaimGroup; 
                db.Claims.Add(claim);
                await db.SaveChangesAsync();
                this.AddNotification("Claims created successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.Roles = db.Roles;
            return View(claims);
        }

        // GET: /UserClaims/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUserClaim claim = await db.AspNetUserClaims.FindAsync(id);
            if (claim == null)
            {
                return HttpNotFound();
            }
            return View(claim);
        }

        // POST: /UserClaims/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ClaimType,ClaimValue,IsActive,Discriminator")] ApplicationUserClaim claims)
        {
            if (ModelState.IsValid)
            {

                var claim = db.AspNetUserClaims.Find(claims.Id);

                claim.ClaimType = claims.ClaimType;
                claim.ClaimValue = claims.ClaimValue;
                claim.IsActive = claims.IsActive;

                await db.SaveChangesAsync();
                this.AddNotification("Claims updated successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
                //var _controller = new AccountController();

                //await _controller.UserManager.RemoveClaimAsync("", claims);


                //await _controller.UserManager.AddClaimAsync("", claims);

                //db.Entry(claims).State = EntityState.Modified;
                //await db.SaveChangesAsync();
                //return RedirectToAction("Index");
            }
            return View(claims);
        }

        // GET: /UserClaims/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Claims claims = await db.Claims.FindAsync(id);
            if (claims == null)
            {
                return HttpNotFound();
            }
            return View(claims);
        }

        // POST: /UserClaims/Delete/5
        [HttpGet, ActionName("DeleteConfirmed")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Claims claims = await db.Claims.FindAsync(id);
            db.Claims.Remove(claims);
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
