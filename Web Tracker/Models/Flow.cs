using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebTracker.Models;

namespace WebTracker.Models
{
    public class Flow
    {
        public int FlowId { get; set; }

        //relations
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual List<FlowData> flowDatas { get; set; } = new List<FlowData>();
        public List<Action> actions { get; set; } = new List<Action>();
    }
}