using SHIVAM_ECommerce.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.ViewModels
{
    public class PlanViewModel : BaseClass
    {

        [Required(ErrorMessage = "Plan Name is Required.")]
        public string PlanName { get; set; }
        public string Plancode { get; set; }

        [Required(ErrorMessage = "Plan Frequency is Required.")]
        public string PlanFrequency { get; set; }

        public decimal MonthlyPrice { get; set; }

        public decimal YearlyPrice { get; set; }
        public int ProductBucketCount { get; set; }

        public bool IsActive { get; set; }

        public int UserBucketCount { get; set; }


        public decimal TotalYearlyPrice { get; set; }

        public decimal TotalMonthlyPrice { get; set; }


        public virtual List<PlanFeaturesViewModel> Features { get; set; }
    }
    public class PlanFeaturesViewModel
    {
        public int ID { get; set; }
        public string title { get; set; }

        public decimal price { get; set; }

        public bool IsSelected { get; set; }
    }
}