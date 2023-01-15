using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebTracker.Models;

namespace WebTracker.Models
{
    public class FlowData
    {
        public int FlowDataId { get; set; }
        public string Page { get; set; }
        
        //it contains foreign key (item for category)
        public int FlowId { get; set; }
        public virtual Flow flow{ get; set; }
    }
}