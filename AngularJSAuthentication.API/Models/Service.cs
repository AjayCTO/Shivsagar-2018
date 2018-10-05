using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AngularJSAuthentication.API.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Display(Name = "Service Name")]
        [Required]
        public string Name { get; set; }           


        [Display(Name = "Some Detail")]
        public string Description { get; set; }

        [Display(Name = "Start Time")]
        public DateTime ServiceStartTime { get; set; }

        [Display(Name = "Estimated Completion Time")]
        public DateTime ServiceCompletionTime { get; set; }

        [Display(Name = "End Time")]
        public DateTime ServiceEndTime { get; set; }

        public bool IsServiceReScheduable { get; set; }

        [Display(Name = "Reschedule Time")]
        public DateTime ServiceRescheduleDate { get; set; }


        public ICollection<Task> Tasks { get; set; }
    }
}