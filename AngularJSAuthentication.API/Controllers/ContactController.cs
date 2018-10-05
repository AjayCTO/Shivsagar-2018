using AngularJSAuthentication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AngularJSAuthentication.API.Controllers
{
    [Route("api/Contact/{action}")]
    public class ContactController : ApiController
    {
        [HttpPost]
        [ActionName("contactus")]
         public HttpResponseMessage contactus(ContactViewModel contact)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string CustomerName = contact.Name;
                    string CustomerMessage = contact.Message;
                    string CustomerEmail = contact.EmailAddress;
                    string adminemail = ConfigurationManager.AppSettings["adminemail"].ToString();
                    string admintemple = ConfigurationManager.AppSettings["admintemple"].ToString();
                    string subject = ConfigurationManager.AppSettings["subject"].ToString();
                    string Thankyou = ConfigurationManager.AppSettings["Thankyou"].ToString();
                    string CustomerContactus = ConfigurationManager.AppSettings["CustomerContactus"].ToString();
                    string AdminUserName = ConfigurationManager.AppSettings["adminusername"].ToString();
                    var sendemail = new EmailService.Service.EmailService();
                    //for cutomer
                    sendemail.SendEmail(CustomerEmail, Thankyou, "Thank you For Contant us", CustomerName,null,null);
                    //for Admin
                    sendemail.SendEmail(adminemail, CustomerContactus, subject, CustomerEmail+" Message:  "+CustomerMessage ,null,null);
                    var result = new
                    {
                       success = true,
                       };
                  return Request.CreateResponse(HttpStatusCode.OK,result);
               
                }
         
             }
            catch (Exception ex)
            {
                var result = new
                {
                    error = ex.InnerException.Message.ToString(),
                    success = false
                };
                return Request.CreateResponse(HttpStatusCode.OK, result);

            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}
