using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.Models
{
    public class WishList
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product product { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer customer { get; set; }
    }

    public class CustomerReviews
    {
        public int Id { get; set; }

        public int ProductId { get; set; } 

        public string Name { get; set; }

        public string Email { get; set; }

        public String Review { get; set; }

        public string Rating { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product product { get; set; }
       

    }
}