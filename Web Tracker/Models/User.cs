using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace WebTracker.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string DeviceType { get; set; } = "";
        public string Browser { get; set; } = "";
        public string OS { get; set; } = ""; 
        public DateTime LastConnection { get; set; }
        public DateTime ReturningData { get; set; }
        public Address Address {get; set; } = new Address();

        //relations
        public virtual List<Flow> Flows { get; set; } = new List<Flow>();
        public int WebsiteId { get; set; }
        public Website Website {get; set; }
    }
}
