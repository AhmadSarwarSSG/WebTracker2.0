using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTracker.Models;
namespace WebTracker.Repositories
{
    public interface IFlowDataRepository
    {
        List<FlowData> GetAllFlowDatas();
        FlowData GetFlowDataById(int id);
        bool AddFlowData(FlowData flowdata);
        bool DeleteFlowData(int id);
        bool UpdateFlowData(int id, FlowData flowData);
    }
}