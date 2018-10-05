using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class emailrecord
    {
        public int Id { get; set; }
        public string Email_Sender { get; set; }
        public string Email_Receiver { get; set; }
        public string Send_Date { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}