using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class Plans : BaseClass
    {
        [Required(ErrorMessage = "Plan Name is Required.")]
        public string PlanName { get; set; }
        public string Plancode { get; set; }

        [Required(ErrorMessage = "Plan Frequency is Required.")]
        [DefaultValue("1")]
        public string PlanFrequency { get; set; }

        public decimal MonthlyPrice { get; set; }

        public decimal YearlyPrice { get; set; }

        public decimal TotalYearlyPrice { get; set; }

        public decimal TotalMonthlyPrice { get; set; }
        public int ProductBucketCount { get; set; }

        public bool IsActive { get; set; }

        public int UserBucketCount { get; set; }
        public virtual List<Supplier> Suppliers { get; set; }

        public virtual List<PlanFeatures> Features { get; set; }
    }

    public class PlanFeatures : BaseClass
    {

        public int PlanID { get; set; }

        public int FeatureID { get; set; }

        [ForeignKey("PlanID")]
        public virtual Plans Plan { get; set; }

        [ForeignKey("FeatureID")]
        public virtual Features Feature { get; set; }

    }
}