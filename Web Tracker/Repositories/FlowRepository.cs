using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebTracker.Models;
using WebTracker.Models;

namespace WebTracker.Repositories
{
    public class FlowRepository :IFlowRepository
    {
        private readonly WebTrackerDBContext _context;

        public FlowRepository(WebTrackerDBContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

        public bool AddFlow(Flow flow)
        {
            _context.Flows.Add(flow);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteFlow(int id)
        {
            var flow = _context.Flows
                        .Where(x => x.FlowId == id)
                        .FirstOrDefault();
            if (flow != null)
            {
                _context.Flows.Remove(flow);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Flow> GetAllFlows() => _context.Flows.ToList();

        public async Task<Flow> GetFlowById(int id) => await _context.Flows.FirstOrDefaultAsync(f => f.FlowId == id);

        public async Task<List<Flow>> GetFlowsByUserId(int userId)
        {
            return await _context.Flows.Where(f => f.UserId == userId).ToListAsync();
        }

        public async Task<List<Flow>> GetFlowsByWebsiteId(int websiteId)
        {
            List<User> users = await _context.Users
                                            .Where(u => u.WebsiteId == websiteId)
                                            .ToListAsync();
            List<Flow> flows = new List<Flow>();
            foreach (User user in users)
            {
                flows.AddRange(await _context.Flows
                                            .Where(f => f.UserId == user.UserId)
                                            .ToListAsync());
            }
            return flows;
        }

        public bool UpdateFlow(int id, Flow flow)
        {
            var flowToUpdate = _context.Flows
                        .Where(x => x.FlowId == id)
                        .FirstOrDefault();
            if (flowToUpdate != null)
            {
                flowToUpdate.UserId = flow.UserId;
                flowToUpdate.User = flow.User;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}