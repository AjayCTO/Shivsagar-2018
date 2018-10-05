using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using paytm;
namespace SHIVAM_ECommerce.Controllers
{
    public class PaytmPaymentController : Controller
    {
        // GET: PaytmPayment
        public ActionResult Index()
        {
            String merchantKey = "merchantKey value";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("MID", "mid value");
            parameters.Add("CHANNEL_ID", "channel id value");
            parameters.Add("INDUSTRY_TYPE_ID", "industry value");
            parameters.Add("WEBSITE", "website value");
            parameters.Add("EMAIL", "email value");
            parameters.Add("MOBILE_NO", "mobile value");
            parameters.Add("CUST_ID", "cust id");
            parameters.Add("ORDER_ID", "order id");
            parameters.Add("TXN_AMOUNT", "amount");
            parameters.Add("CALLBACK_URL", "url");
            string checksum = CheckSum.generateCheckSum(merchantKey, parameters);
            parameters.Add("CHECKSUMHASH", checksum);
            return Json(parameters);
        }
    }
}