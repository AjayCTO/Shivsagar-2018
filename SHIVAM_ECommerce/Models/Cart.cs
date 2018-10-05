using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string UserID { get; set; }
        public int CustomerId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product product { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer customer { get; set; }
    }
}