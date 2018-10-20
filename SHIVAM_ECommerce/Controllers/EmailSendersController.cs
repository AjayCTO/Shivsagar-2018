using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using SHIVAM_ECommerce.Models;
using Microsoft.AspNet.Identity;
using SHIVAM_ECommerce.Extensions;
using System.IO;
   
using SHIVAM_ECommerce.ViewModels;
namespace SHIVAM_ECommerce.Controllers
{
       
    public class EmailSendersController : BaseController
    {
       private ApplicationDbContext db = new ApplicationDbContext();


       public ActionResult Index()
       {

           try
           {

               var ctx = new ApplicationDbContext();
               var _List = new List<EmailDetail>();
               using (var cmd = ctx.Database.Connection.CreateCommand())
               {
                   ctx.Database.Connection.Open();
               
                   cmd.CommandText = "select * from GetAllEmails where userId='" + User.Identity.GetUserId() + "'";
                   using (var reader = cmd.ExecuteReader())
                   {

                       while (reader.Read())
                       {
                           _List.Add(new EmailDetail()
                           {

                               Id = Convert.ToInt32(reader["Id"]),
                               Attachment = reader["Attachment"].ToString(),
                               ContentMsg = reader["ContentMsg"].ToString(),
                               IsAttachment = Convert.ToBoolean(reader["IsAttachment"]),
                               SenderId = reader["SenderId"].ToString(),
                               sentUsers = reader["ReceiverId"].ToString(),
                               individualId = Guid.Parse(reader["userId"].ToString()),
                               Subject = reader["Subject"].ToString(),
                               SendingDate = reader["SendingDate"].ToString()

                           });
                       }
                     
                       return View(_List);
                   }
               }

           }
           catch (Exception ex)
           {

               return View(new List<EmailDetail>());
           }
       }
      

                public ActionResult LoadSentData()
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
              var user_name= User.Identity.GetUserName();
              var Emailrecorddata = db.EmailRecord.Where(x => x.Email_Sender == user_name).ToList();
            // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
              var v = (from a in Emailrecorddata select a);
            if (!string.IsNullOrEmpty(searchitem))
            {

                v = v.Where(b => b.Email_Sender.ToLower().Contains(searchitem.ToLower()));
            }
            //SORT
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                v = v.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = v.Count();
            var data = v.Skip(skip).Take(pageSize).ToList();
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(p => new { id = p.Id, ReceiverName=p.ReceiverName, Email_Sender = p.Email_Sender, Email_Receiver = p.Email_Receiver, Send_Date = p.Send_Date, Subject = p.Subject, Message = p.Message, IsAttachment = p.IsAttachment, IsRead = p.IsRead, Attachment = p.Attachment }) }, JsonRequestBehavior.AllowGet);
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
           var ctx = new ApplicationDbContext();
           var _List = new List<EmailDetail>();
           using (var cmd = ctx.Database.Connection.CreateCommand())
           {
               ctx.Database.Connection.Open();

               cmd.CommandText = "select * from GetAllEmails where userId='" + User.Identity.GetUserId() + "'";
               using (var reader = cmd.ExecuteReader())
               {

                   while (reader.Read())
                   {
                       _List.Add(new EmailDetail()
                       {

                           Id = Convert.ToInt32(reader["Id"]),
                           Attachment = reader["Attachment"].ToString(),
                           ContentMsg = reader["ContentMsg"].ToString(),
                           IsAttachment = Convert.ToBoolean(reader["IsAttachment"]),
                           SenderId = reader["SenderId"].ToString(),
                           sentUsers = reader["ReceiverId"].ToString(),
                           individualId = Guid.Parse(reader["userId"].ToString()),
                           Subject = reader["Subject"].ToString(),
                           SendingDate = reader["SendingDate"].ToString()

                       });
                   }

               }
           }





           var v = (from a in _List select a);
           if (!string.IsNullOrEmpty(searchitem))
           {

               v = v.Where(b => b.SenderId.ToLower().Contains(searchitem.ToLower()) || b.Subject.ToLower().Contains(searchitem.ToLower()) || b.SendingDate.ToLower().Contains(searchitem.ToLower()) || b.ContentMsg.ToLower().Contains(searchitem.ToLower()));
           }
           //SORT
           if (!(string.IsNullOrEmpty(sortColumn) || sortColumn == "" && string.IsNullOrEmpty(sortColumnDir)))
           {
               v = v.OrderBy(sortColumn + " " + sortColumnDir);
           }

           recordsTotal = v.Count();
           var data = v.Skip(skip).Take(pageSize).ToList();
           return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data.Select(x => new { x.Id, x.Subject, x.SenderId,x.Attachment,x.SendingDate,x.ContentMsg,x.IsAttachment,x.IsRead }) }, JsonRequestBehavior.AllowGet);
       }


        public ActionResult parital()
        {
            try
            {

                var ctx = new ApplicationDbContext();
                var _List = new List<EmailDetail>();
                using (var cmd = ctx.Database.Connection.CreateCommand())
                {
                    ctx.Database.Connection.Open();
                    cmd.CommandText = "select * from GetAllEmails where userId='" + User.Identity.GetUserId() + "'";
                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            _List.Add(new EmailDetail()
                            {

                                Id = Convert.ToInt32(reader["Id"]),
                                SenderId = reader["SenderId"].ToString(),
                                Subject = reader["Subject"].ToString(),
                                SendingDate = reader["SendingDate"].ToString(),
                                IsRead = Convert.ToBoolean(reader["IsRead"]),

                            });
                        }

                        return View(_List);
                    }
                }

            }
            catch (Exception ex)
            {

                return View(new List<EmailDetail>());
            }

        }


        // GET: /EmailSenders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailSender emailsender = await db.EmailSenders.FindAsync(id);
            if (emailsender == null)
            {
                return HttpNotFound();
            }
            return View(emailsender);
        }

        // GET: /EmailSenders/Create
        public ActionResult Create()
        {
           var subject= TempData["Subject"];
           ViewBag.EmailSubject = subject;
           return View();
        }

    [HttpPost]
        public ActionResult Subject(string subject)
        {
            TempData["Subject"] = subject;
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
          }
        [HttpPost]
        public JsonResult GetUser(string term)
        {
        
          
            var User = db.Users.Where(x => x.UserName.StartsWith(term)).ToList();
            var data = User.Select(x => new { x.UserName, x.Id });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        // POST: /EmailSenders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ReceiverId,SenderId,Subject,ContentMsg,IsAttachment,IsRead,Attachment,SendingDate,ReceiverIDs")] EmailSender emailsender, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                emailrecord obj= new emailrecord();
              
              string s = emailsender.ReceiverIDs;
     
               string[] words = s.Split(',');
                foreach (string word in words)
                 {
           
      
                    if (file != null)
                    {
                        emailsender.IsAttachment = true;
                        obj.IsAttachment = true;
                       
                        string pic = System.IO.Path.GetFileName(file.FileName);
                        string path = System.IO.Path.Combine(
                                               Server.MapPath("~/ProductImages"), pic);
                        // file is uploaded
                        file.SaveAs(path);
                        obj.Attachment = pic;
                        emailsender.Attachment = pic;

                        // save the image path path to the database or you can send image 
                        // directly to database
                        // in-case if you want to store byte[] ie. for DB
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                        }
                    }

                    emailsender.ReceiverId = word;
                    var name = db.Users.Where(x => x.Id == word).FirstOrDefault();
                    emailsender.SenderId = User.Identity.GetUserName();
                    emailsender.SenderIds = User.Identity.GetUserId();
                    emailsender.SendingDate = DateTime.Now.ToString();
                    obj.Subject = emailsender.Subject;
                    obj.Email_Sender = emailsender.SenderId;
                    obj.Message = emailsender.ContentMsg;
                    obj.Send_Date = emailsender.SendingDate;
                    obj.Email_Receiver = word;
                    obj.ReceiverName = name.UserName;

                    db.EmailRecord.Add(obj);
                  await  db.SaveChangesAsync();
                  
                    db.EmailSenders.Add(emailsender);

                    await db.SaveChangesAsync();
                    



                 }

                this.AddNotification("Send successfully.", NotificationType.SUCCESS);
                return RedirectToAction("Index", "EmailSenders");
                 
            }

            return View(emailsender);
        }

        // GET: /EmailSenders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailSender emailsender = await db.EmailSenders.FindAsync(id);
            if (emailsender == null)
            {
                return HttpNotFound();
            }
            return View(emailsender);
        }

    [HttpPost]
        public async Task<ActionResult> EditIsRead(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }  
            EmailSender emailsender = await db.EmailSenders.FindAsync(id);
            emailsender.IsRead = true;
            db.Entry(emailsender).State = EntityState.Modified;
            await db.SaveChangesAsync();
           if(emailsender==null)
            {
                return HttpNotFound();
            }
           return View(emailsender);
        }


    public async Task<ActionResult> UniqueDetailView(int? id)
    {
        if(id==null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
    
        EmailSender emailsender = await db.EmailSenders.FindAsync(id);
        var next = db.EmailSenders.Where(t => t.Id > id).OrderBy(t => t.Id).Take(1).FirstOrDefault();
        ViewBag.nextrecord = next;
        var prev = db.EmailSenders.Where(t => t.Id < id).OrderByDescending(t=>t.Id).Take(1).FirstOrDefault();
        ViewBag.Prevrecord = prev;
        if(emailsender==null)
        {
            return HttpNotFound();
        }
        return View(emailsender);
    }
        public async Task<ActionResult> Reply(int? id)
        {
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            EmailSender emailsender = await db.EmailSenders.FindAsync(id);
      
    
            var User = emailsender.SenderId; 
            var userlist= db.Users.Where(x => x.UserName.StartsWith(User)).ToList();
            emailsender.SenderIds = userlist.Select(x => x.Id).FirstOrDefault();
            if(emailsender==null)
            {
                return HttpNotFound();
            }
            return View(emailsender);
        }

        public async Task<ActionResult> Forward(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            EmailSender emailsender = await db.EmailSenders.FindAsync(id);
       
            if (emailsender == null)
            {
                return HttpNotFound();
            }
            return View(emailsender);
        }
        public ActionResult Download(string name)
        {


            string fileName = name;

            string folder = Server.MapPath("/ProductImages");
            folder = folder.Replace("EmailSenders", "");
            //HttpContext.Response.AddHeader("content-dispostion", "attachment; filename=" + fileName);
            return File(new FileStream(folder + "/" + fileName, FileMode.Open), "content-dispostion", name);

            throw new ArgumentException("Invalid file name of file not exist");
        }

        public ActionResult Sent()
        {
            return View();
        }

        // POST: /EmailSenders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,ReceiverId,SenderId,Subject,ContentMsg,IsAttachment,Attachment,IsRead,SendingDate")] EmailSender emailsender)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emailsender).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(emailsender);
        }

        //[HttpPost]
        //public ActionResult DownloadFile()
        //{
        //    string path = AppDomain.CurrentDomain.BaseDirectory + "FolderName/";
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(path + "filename.extension");
        //    string fileName = "filename.extension";
        //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        //}



        // GET: /EmailSenders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmailSender emailsender = await db.EmailSenders.FindAsync(id);
            if (emailsender == null)
            {
                return HttpNotFound();
            }
            return View(emailsender);
        }

     [HttpPost]
        public async Task<ActionResult> Deleteall(List<int> Ids)
              {
   
            foreach (int id  in Ids)
             {
            EmailSender obj = await db.EmailSenders.FindAsync(id);
            db.EmailSenders.Remove(obj);
             }
            await db.SaveChangesAsync();
             return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
                 }



        // POST: /EmailSenders/Delete/5
    [HttpPost]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            EmailSender emailsender = db.EmailSenders.Where(x => x.Id == id).FirstOrDefault();
            db.EmailSenders.Remove(emailsender);
            await db.SaveChangesAsync();
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

    [HttpPost]
    public async Task<ActionResult> DeleteSentConfirmed(int id)
    {
        emailrecord emailrecord = db.EmailRecord.Where(x => x.Id == id).FirstOrDefault();
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


