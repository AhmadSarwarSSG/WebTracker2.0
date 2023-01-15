using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebTracker.Models
{
    public class Website
    {
        public int WebsiteId { get; set; }
        public string Web { get; set; } = "";
        public int VisitCount { get; set; }
        public virtual List<User> Users { get; set; } = new List<User>();

        
    }
}