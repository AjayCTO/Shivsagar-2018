using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class Payment : BaseClass
    {
        #region Properties
        public decimal Amount { get; set; }
        public string ReferenceDetails { get; set; }
        public string Notes { get; set; }

        public string PaymentType { get; set; }

        public string PaymentStatus { get; set; }
        #endregion


    }
}