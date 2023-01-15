using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignalRCheck;
using Microsoft.EntityFrameworkCore;
using WebTracker.Repositories;
using WebTracker.Models;

namespace WebTracker.Repositories
{
    public class FlowDataRepository : IFlowDataRepository
    {
        private readonly WebTrackerDBContext _context;
        public FlowDataRepository(WebTrackerDBContext context) => _context = context;
        public bool AddFlowData(FlowData flowData)
        {
            _context.FlowDatas.Add(flowData);
            _context.SaveChanges();
            return true;
        }
        public bool DeleteFlowData(int id)
        {
            var flowdatas = _context.FlowDatas
                        .Where(x => x.FlowDataId == id)
                        .FirstOrDefault();
            if (flowdatas != null)
            {
                _context.FlowDatas.Remove(flowdatas);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public FlowData GetFlowDataById(int id)
        {
            return _context.FlowDatas.FirstOrDefault(a => a.FlowDataId == id);
        }

        public List<FlowData> GetAllFlowDatas()
        {
            return _context.FlowDatas.ToList();
        }

        public bool UpdateFlowData(int id, FlowData flowdatas)
        {
            var flowdataToUpdate = _context.FlowDatas.FirstOrDefault(a => a.FlowDataId == id);
            if (flowdataToUpdate != null)
            {
                flowdataToUpdate.Page = flowdatas.Page;
                flowdataToUpdate.FlowId = flowdatas.FlowId;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}