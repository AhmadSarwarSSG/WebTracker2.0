using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebTracker.Models
{
    public class Action
    {
        public int ActionId { get; set; }
        public string Type { get; set; } = "";
        public string Content { get; set; } = "";
        public int FlowId { get; set; }
        public string Page { get; set; } = "";
        public Flow flow { get; set; }
    }
}