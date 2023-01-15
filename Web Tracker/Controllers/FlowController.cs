using Microsoft.AspNetCore.Mvc;
using WebTracker.Controllers;
using WebTracker.Repositories;
using SignalR_Check;
using WebTracker.Models;
using SignalR_Check.Models;

namespace SignalR_Check.Controllers
{
    public class FlowController : Controller
    {
        IWebsiteRepository _websiteRepository;
        IUserRepository _userRepository;
        IFlowRepository _flowRepository;
        IActionRepository _actionRepository;
        IFlowDataRepository _flowDataRepository;
        ISummaryData _summaryData;
        public FlowController(IWebsiteRepository websiteRepository, IUserRepository userRepository, IFlowRepository flowRepository, IActionRepository actionRepository, IFlowDataRepository flowDataRepository, ISummaryData summaryData)
        {
            _websiteRepository = websiteRepository;
            _userRepository = userRepository;
            _flowRepository = flowRepository;
            _actionRepository = actionRepository;
            _flowDataRepository = flowDataRepository;
            _summaryData = summaryData;
        }
        public IActionResult Index()
        {
            var flow = _flowDataRepository.GetAllFlowDatas()
                .Join(_flowRepository.GetAllFlows(), p => p.FlowId, u => u.FlowId, (p, u) => new { p, u })
                .Join(_userRepository.GetAllUsers(), pu => pu.u.UserId, c => c.UserId, (pu, c) => new { pu.p, pu.u, c })
                .Where(puc => puc.c.WebsiteId == 5).Select(puc => new { puc.p.Page, puc.p.FlowId}).ToList();
            List<int> uniqnum=flow.Select(u=>u.FlowId).Distinct().ToList();
            List<string> contFlows = new List<string>();
            string cf = "";
            foreach (var f in uniqnum)
            {
                foreach (var fd in flow)
                {
                    if (fd.FlowId == f)
                    {
                        if (fd.Page == "/")
                        {
                            cf = cf + "Landing Page" + " ➜ ";
                        }
                        else
                        {
                            cf = cf + fd.Page + " ➜ ";
                        }
                    }
                }
                contFlows.Add(cf);
                cf = "";
            }
            ViewBag.Flows = contFlows;
            ViewBag.FlowIDs = uniqnum;
            var Action = _actionRepository.GetAllActions()
                .Join(_flowRepository.GetAllFlows(), p => p.FlowId, u => u.FlowId, (p, u) => new { p, u })
                .Join(_userRepository.GetAllUsers(), pu => pu.u.UserId, c => c.UserId, (pu, c) => new { pu.p, pu.u, c })
                .Where(puc => puc.c.WebsiteId == 5).Select(puc => new { puc.p.Page, puc.p.FlowId, puc.p.Type,puc.p.Content}).ToList();
            List<string> uniqpage = Action.Select(u => u.Page).Distinct().ToList();
            List<string> contActions = new List<string>();
            List<List<string>> pageactions=new List<List<string>>();
            string pc = "";
            foreach(var f in uniqnum)
            {
                List<string> list = new List<string>();
                foreach (var p in uniqpage)
                {
                    bool first = true;
                    string temp = pc;
                    foreach(var a in Action)
                    {
                        if(a.FlowId==f && a.Page==p)
                        {
                            if (a.Content=="/")
                            {
                                if (first)
                                {
                                    first=false;
                                }
                                pc = pc + a.Type + "(" + "Landing Page" + ") ➜ ";
                            }
                            else
                            {
                                if (first)
                                {
                                    first = false;
                                }
                                pc = pc + a.Type + "(" + a.Content + ") ➜ ";
                            }
                        }
                    }
                    if(pc!=temp)
                    {
                        list.Add(pc);
                    }
                }
                pageactions.Add(list);
                //contActions.Add(pc);
                pc = "";
            }
            ViewBag.Actions = pageactions;
            ViewBag.pages = uniqpage;
            List<FlowSummary> flows = _summaryData.GetAllFlows();
            foreach(var f in flows)
            {
                string finalFlow = "";
                for(int i=0; i < f.FlowSummed.Length; i++)
                {
                    if (f.FlowSummed[i] == '>')
                    {
                        finalFlow += "➜";
                    }
                    else if(i==f.FlowSummed.Length-1 && f.FlowSummed[i] == '/')
                    {
                        finalFlow += "LandingPage";
                    }
                    else if (f.FlowSummed[i]=='/' && f.FlowSummed[i+1]=='>')
                    {
                        finalFlow += "LandingPage";
                    }
                    else
                    {
                        finalFlow += f.FlowSummed[i];
                    }
                }
                f.FlowSummed = finalFlow;
            }
            ViewBag.Summary = flows;
            return View();
        }
    }
}
