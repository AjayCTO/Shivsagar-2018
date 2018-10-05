using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class Log:BaseClass
    {



        public DateTime ErrorDate { get; set; }
        public string RequestedURL { get; set; }
        public string MachineName { get; set; }
        public string PhysicalAppPath { get; set; }
        public string Browser { get; set; }

        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string LoggerName { get; set; }
        public string LoggerLevel { get; set; }
        public string Message { get; set; }

        public string IP { get; set; }
        public bool IsSecure { get; set; }

    }
}