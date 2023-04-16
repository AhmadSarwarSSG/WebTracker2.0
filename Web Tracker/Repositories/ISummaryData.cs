using SignalR_Check.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTracker.Models;

namespace WebTracker.Repositories
{
    public interface ISummaryData
    {
        List<FlowSummary> GetAllFlows(int loggedInUserWebsiteId);
        List<ActionSummary> GetAllActions(int loggedInUserWebsiteId);
    }
}