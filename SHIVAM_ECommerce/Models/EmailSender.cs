using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SHIVAM_ECommerce.Models
{
    public class EmailSender
    {
        [Key]
       public int Id { get; set; }
       [Required]
       public string ReceiverId { get; set; }
       public string SenderId { get; set; }
       public string Subject { get; set; }
        [AllowHtml]
       public string ContentMsg { get; set; }
       public bool IsAttachment { get; set; }
       public string Attachment { get; set; }
       public bool IsRead { get; set; }
       public string SendingDate { get; set; }

        [NotMapped]
       public string ReceiverIDs { get; set; }
        [NotMapped]
        public string SenderIds { get; set; }
    }
}