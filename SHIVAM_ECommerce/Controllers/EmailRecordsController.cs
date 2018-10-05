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
using System.Linq.Dynamic;
namespace SHIVAM_ECommerce.Controllers
{
    public class EmailRecordsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /EmailRecords/
        public async Task<ActionResult> Index()
        {
            return View(await db.EmailRecord.ToListAsync());
        }

        // GET: /EmailRecords/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            emailrecord emailrecord = await db.EmailRecord.FindAsync(id);
            if (emailrecord == null)
            {
                return HttpNotFound();
            }
            return View(emailrecord);
        }

        // GET: /EmailRecords/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /EmailRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Email_Sender,Email_Receiver,Send_Date,Subject,Message")] emailrecord emailrecord)
        {
            if (ModelState.IsValid)
            {
                db.EmailRecord.Add(emailrecord);
                await db.SaveChangesAsync();
                this.AddNotification("Email record successfully.", NotificationType.SUCCESS);

                return RedirectToAction("Index");
            }

            return View(emailrecord);
        }
        //[HttpPost]
        //public ActionResult GetAllEmailrecord()
        //{
        //    var AllEmailRecords=db.EmailRecord.Select(p=>new{sender=p.Email_Sender,receiver=p.Email_Receiver,senddate=p.Send_Date,subject=p.Subject,message=p.Message}).ToList();
        //    return Json(AllEmailRecords, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult GetAllEmailrecord()
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
            var v = (from a in db.EmailRecord select a);
            if (!string.IsNullOrEmpty(searchitem))
            {

                v = v.Where(b => b.Subject.Contains(searchitem) || b.Message.Contains(searchitem) || b.Email_Receiver.Contains(searchitem) || b.Email_Sender.Contains(searchitem));
            }

            switch (sortColumn)
            {
                case "id":
                    sortColumn = "Id";
                    break;
                case "sender":
                    sortColumn = "Email_Sender";
                    break;
                case "receiver":
                    sortColumn = "Email_Receiver";
                    break;
                case "senddate":
                    sortColumn = "Send_Date";
                    break;
                case "subject":
                    sortColumn = "Subject";
                    break;
                case "message":
                    sortColumn = "Message";
                    break;
                default:
                    break;
            }


            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(p => new { id=p.Id,sender = p.Email_Sender, receiver = p.Email_Receiver, senddate = p.Send_Date, subject = p.Subject, message = p.Message }) }, JsonRequestBehavior.AllowGet);
        }

        // GET: /EmailRecords/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            emailrecord emailrecord = await db.EmailRecord.FindAsync(id);
            if (emailrecord == null)
            {
                return HttpNotFound();
            }
            return View(emailrecord);
        }

        // POST: /EmailRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Email_Sender,Email_Receiver,Send_Date,Subject,Message")] emailrecord emailrecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emailrecord).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(emailrecord);
        }

        // GET: /EmailRecords/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            emailrecord emailrecord = await db.EmailRecord.FindAsync(id);
            if (emailrecord == null)
            {
                return HttpNotFound();
            }
            return View(emailrecord);
        }

        // POST: /EmailRecords/Delete/5
        [HttpPost, ActionName("Delete")]
 
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            emailrecord emailrecord = await db.EmailRecord.FindAsync(id);
            db.EmailRecord.Remove(emailrecord);
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
