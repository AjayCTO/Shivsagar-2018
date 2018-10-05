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
using SHIVAM_ECommerce.Attributes;
using System.Linq.Dynamic;
using SHIVAM_ECommerce.Extensions;
namespace SHIVAM_ECommerce.Controllers
{
    [CustomAuthorize]
    public class OrderController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Order/
        public async Task<ActionResult> Index()
        {

            return View();
        }

        [HttpGet]
        public ActionResult SearchSupplier(string SearchValue)
        {
            try
            {
                var suppliers = db.Suppliers.Where(p => p.FirstName.ToLower().Contains(SearchValue.ToLower()) || p.LastName.ToLower().Contains(SearchValue.ToLower())).Select(p => new { SupplierName = p.FirstName + " " + p.LastName, SupplierID = p.Id });
                return Json(suppliers.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
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
            var _supplierID = !string.IsNullOrEmpty(Request["SupplierID"]) ? Convert.ToInt16(Request["SupplierID"]) : CurrentUserData.SupplierID;

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            var _orderIds = db.OrderItems.Where(x => x.SupplierID == _supplierID).Select(x => x.Orders_Id);
            var orders = _supplierID == -1 ? db.Orders.Include(o => o.customer) : db.Orders.Include(o => o.customer).Where(x => _orderIds.Contains(x.Id));
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


        public ActionResult GetOrderItems(int orderID)
        {
            try
            {
                var _orderItems = db.OrderItems.Where(x => x.SupplierID == CurrentUserData.SupplierID && x.Orders_Id == orderID).ToList();
                return PartialView("OrderItems", _orderItems);
            }
            catch (Exception ex)
            {

                return Json(new { success = false, ex = ex.Message.ToString(), data = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /Order/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = await db.Orders.FindAsync(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // GET: /Order/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "Id", "FirstName");
            return View();
        }

        // POST: /Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,OrderNumber,OrderDate,ShipDate,RequiredDate,IsPaid,TimeStamp,Isdeleted,TransactionStatus,CustomerID,TotalDiscount,CreatedDate,UpdatedDate,Sort,Description,Notes")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(orders);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "Id", "FirstName", orders.CustomerID);
            return View(orders);
        }

        // GET: /Order/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = await db.Orders.FindAsync(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "Id", "FirstName", orders.CustomerID);
            return View(orders);
        }

        // POST: /Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,OrderNumber,OrderDate,ShipDate,RequiredDate,IsPaid,TimeStamp,Isdeleted,TransactionStatus,CustomerID,TotalDiscount,CreatedDate,UpdatedDate,Sort,Description,Notes")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orders).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "Id", "FirstName", orders.CustomerID);
            return View(orders);
        }

        [HttpPost]
        public async Task<ActionResult> UpdatePaymentStatus(int ID)
        {
            try
            {
                await Task.Delay(100);
                var _order = db.Orders.Where(x => x.Id == ID).FirstOrDefault();
                _order.IsPaid = true;
                db.SaveChanges();
                return Json(new { Success = true, ex = "" });

            }
            catch (Exception ex)
            {
                return Json(new { Success = true, ex = ex.Message.ToString() });
            }
        }


        // GET: /Order/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = await db.Orders.FindAsync(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: /Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Orders orders = await db.Orders.FindAsync(id);
            db.Orders.Remove(orders);
            await db.SaveChangesAsync();
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
