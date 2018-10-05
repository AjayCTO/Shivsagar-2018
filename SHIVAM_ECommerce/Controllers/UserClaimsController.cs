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
using SHIVAM_ECommerce.Repository;
using Microsoft.AspNet.Identity.EntityFramework;
using SHIVAM_ECommerce.Attributes;
using System.Linq.Dynamic;
using SHIVAM_ECommerce.Extensions;
namespace SHIVAM_ECommerce.Controllers
{
    [CustomAuthorize]
    public class UserClaimsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private IRepository<Claims> _repository = null;
        public UserClaimsController()
        {
            this._repository = new Repository<Claims>();
        }



        // GET: /UserClaims/
        public async Task<ActionResult> Index()
        {

            return View(await db.Claims.ToListAsync());
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

                v = v.Where(b => b.Role.Contains(searchitem)).ToList();
            }
            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.ClaimType, x.ClaimValue, x.IsActive, x.CreatedDate, x.UpdatedDate, x.Sort, x.Description, x.Notes, x.Role }) }, JsonRequestBehavior.AllowGet);
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
            ViewBag.AllClaims = db.Claims.Where(x => x.Role == "SuperAdmin");


            ViewBag.Roles = db.Roles;

            return View();
        }

        // POST: /UserClaims/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ClaimType,ClaimValue,IsActive,CreatedDate,UpdatedDate,Sort,Description,Notes,Role")] Claims claims)
        {
            claims.CreatedDate = DateTime.Now;
            claims.UpdatedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Claims.Add(claims);
                await db.SaveChangesAsync();
                this.AddNotification("User Claims Created successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.Roles = db.Roles;
            return View(claims);
        }



        public ActionResult CreateFromAjax(List<Claims> model)
        {
            if (ModelState.IsValid)
            {

                var RoleName = model[0].Role;

                List<Claims> existingClaims = db.Claims.Where(x => x.Role.ToLower() == RoleName.ToLower()).ToList();

                foreach (var claim in model)
                {

                    var _obj = existingClaims.Where(x => x.ClaimValue.ToLower() == claim.ClaimValue.ToLower()).FirstOrDefault();

                    if (_obj == null && claim.IsActive == true)
                    {
                        claim.CreatedDate = DateTime.Now;
                        claim.UpdatedDate = DateTime.Now;

                        db.Claims.Add(claim);
                    }
                    else if (_obj != null)
                    {
                        _obj.IsActive = claim.IsActive;
                    }

                }
                db.SaveChanges();
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

            }

            ViewBag.Roles = db.Roles;
            return View(model);

            //claims.CreatedDate = DateTime.Now;
            //claims.UpdatedDate = DateTime.Now;

            //if (ModelState.IsValid)
            //{
            //    db.Claims.Add(claims);
            //    await db.SaveChangesAsync();
            //    return RedirectToAction("Index");
            //}
            //ViewBag.Roles = db.Roles;
            //return View(model);
        }




        // GET: /UserClaims/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            ViewBag.Roles = db.Roles;
            return View(claims);
        }

        // POST: /UserClaims/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ClaimType,ClaimValue,IsActive,CreatedDate,UpdatedDate,Sort,Description,Notes,Role")] Claims claims)
        {
            if (ModelState.IsValid)
            {

                db.Entry(claims).State = EntityState.Modified;
                await db.SaveChangesAsync();
                this.AddNotification("User Claims Updated successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.Roles = db.Roles;
            return View(claims);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                Claims claims = db.Claims.Find(id);

                var _ClaimsData = claims != null ? db.AspNetUserClaims.Where(x => x.ClaimValue.ToLower() == claims.ClaimValue.ToLower()).Count() : 0;
                if (_ClaimsData == 0 && claims != null)
                {
                    db.Claims.Remove(claims);
                    db.SaveChanges();
                    return Json(new { Success = true, ex = "" });
                }
                else
                {
                    return Json(new { Success = false, ex = "This Claim is Associated with some User, unable to delete this." });
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
