using SHIVAMFaceEcomm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHIVAMFaceEcomm.Controllers
{
    public class CategoryViewModel
    {
        
    
        public int Id { get; set; }
        public string CategoryName { get; set; }
         
        public Nullable<int> ParentCategory { get; set; }
        public string CategoryImage { get; set; }
        public string Description { get; set; }

        public List<CategoryViewModel> Categories1 { get; set; }
        
    }

    public class WishListItems
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }
        public Product Product { get; set; }
    }
}
