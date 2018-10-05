using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SHIVAM_ECommerce.ViewModels
{
    public class EmailDetail
    {
        public int Id { get; set; }

        public string sentUsers { get; set; }
        public Guid individualId { get; set; }
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public string Subject { get; set; }
        public string ContentMsg { get; set; }
        public bool IsAttachment { get; set; }
        public string Attachment { get; set; }
        public bool IsRead { get; set; }
        public string SendingDate { get; set; }
    }
}