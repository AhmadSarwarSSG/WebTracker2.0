using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTracker.Models;
namespace WebTracker.Repositories
{
    public interface IFlowRepository
    {
        List<Flow> GetAllFlows();
        Task<List<Flow>> GetFlowsByWebsiteId(int websiteId);
        Task<List<Flow>> GetFlowsByUserId(int userId);
        Task<Flow> GetFlowById(int id);
        bool AddFlow(Flow flow);
        bool DeleteFlow(int id);
        bool UpdateFlow(int id, Flow flow);
    }
}