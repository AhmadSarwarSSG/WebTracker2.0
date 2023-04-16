using Microsoft.AspNetCore.SignalR;
using System;
using WebTracker.Models;
using WebTracker.Repositories;
using Newtonsoft.Json.Linq;
using WebTracker.Controllers;
using Microsoft.AspNetCore.Mvc;
using SignalR_Check.Models;
using Microsoft.AspNetCore.Identity;

namespace WebTracker.Hubs
{
    public class ClientHub : Hub
    {
        IWebsiteRepository _websiteRepository;
        IUserRepository _userRepository;
        IFlowRepository _flowRepository;
        IActionRepository _actionRepository;
        IFlowDataRepository _flowDataRepository;
        ISummaryData _summaryData;

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public static int loggedInUserWebsiteId;
        public ClientHub(IWebsiteRepository websiteRepository, IUserRepository userRepository, IFlowRepository flowRepository, IActionRepository actionRepository, IFlowDataRepository flowDataRepository, ISummaryData summaryData, UserManager<ApplicationUser> uManager, SignInManager<ApplicationUser> sManager)
        {
            Console.WriteLine("\n\n\n --------------client hub constructor called----------------");

            _websiteRepository = websiteRepository;
            _userRepository = userRepository;
            _flowRepository = flowRepository;
            _actionRepository = actionRepository;
            _flowDataRepository = flowDataRepository;
            _summaryData = summaryData;
            //loggedInUserWebsiteId = -5;
            userManager = uManager;
            signInManager = sManager;
        }

        public void SaveWebsiteId(string siteId)
        {
            Console.WriteLine("site iddd " + siteId);
            //Console.WriteLine("site iddd " + typeof(siteId));

            loggedInUserWebsiteId = int.Parse(siteId);
            Console.WriteLine("----%% $$$$ save website id method from account controlller, in hub id after: " + loggedInUserWebsiteId);
            Console.WriteLine("****************************************************\n\n\n");
        }
        public async Task AddNewUser(string web, string url, string deviceType, string browser, string os, string location, string OS)
        {
            if (userManager.Users.Where(p => p.WebsiteUrl == web).Count() == 0)
            {
                return;
            }
            await Clients.Caller.SendAsync("GetWebsiteId");
            Console.WriteLine("--- website id in add new user %%%%%%%%%%%%%%%%%%%%% " + loggedInUserWebsiteId);

            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@  Hub: New User Connected to " + web + " at " + url + " with " + browser + " using " + deviceType + " from " + location);
            // check if website is in the database
            int websiteId = _websiteRepository.GetWebsiteIdByName(web);
            // add or update website in database
            Website website;
            if (websiteId == -1)
            {
                website = new Website()
                {
                    Web = web,
                    VisitCount = 1
                };
                try
                {
                    _websiteRepository.AddWebsite(website);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await Task.CompletedTask;
                    return;
                }
                websiteId = website.WebsiteId;
            }
            else
            {
                website = await _websiteRepository.GetWebsiteById(websiteId);
                website.VisitCount++;

                try
                {
                    _websiteRepository.UpdateWebsite(websiteId, website);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await Task.CompletedTask;
                    return;
                }

            }

            // add new user in the database
            JObject userLocation = JObject.Parse(location);
            User user = new User()
            {
                DeviceType = deviceType,
                Browser = browser,
                OS = os,
                LastConnection = DateTime.Now,
                Address = new Address()
                {
                    CountryCode = userLocation["country_code"].ToString(),
                    CountryName = userLocation["country_name"].ToString(),
                    City = userLocation["city"].ToString(),
                    Postal = userLocation["postal"].ToString(),
                    Latitude = userLocation["latitude"].ToString(),
                    Longitude = userLocation["longitude"].ToString(),
                    IPv4 = userLocation["IPv4"].ToString(),
                    State = userLocation["state"].ToString()
                },
                WebsiteId = websiteId
            };
            try
            {
                _userRepository.AddUser(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _websiteRepository.DeleteWebsite(websiteId);
                await Task.CompletedTask;
                return;
            }
            int totalvisitors = _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId).Count();
            await Clients.All.SendAsync("TotalVisitors", totalvisitors);
            int newvisitors = _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId && u.ReturningData.Year == 0001).Count();
            await Clients.All.SendAsync("NewVisitors", newvisitors);
            await Clients.All.SendAsync("DeviceData", _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId).GroupBy(u => u.DeviceType).Select(group => new
            {
                name = group.Key,
                value = group.Count(),
            }).OrderBy(x => x.name).ToList());
            await Clients.All.SendAsync("OSData", _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId).GroupBy(u => u.OS).Select(group => new
            {
                country = group.Key,
                value = group.Count(),
            }).OrderBy(x => x.country).ToList());
            await Clients.All.SendAsync("BroswerData", _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId).GroupBy(u => u.Browser).Select(group => new
            {
                country = group.Key,
                value = group.Count(),
            }).OrderBy(x => x.country).ToList());
            await Clients.All.SendAsync("VisitorData", _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId).GroupBy(u => u.LastConnection.ToString("yyyy-MM-dd")).Select(group => new
            {
                date = group.Key,
                value = group.Count(),
            }).OrderBy(x => x.date).ToList());
            var allactions = _actionRepository.GetAllActions();
            var allflows = _flowRepository.GetAllFlows();
            var allad = _userRepository.GetAddresses();
            var allusers = _userRepository.GetAllUsers();
            await Clients.All.SendAsync("RegionData", allusers.Join(allad,
                a => a.Address.AddressId,
                b => b.AddressId,
                (a, b) => new {
                    a.WebsiteId,
                    b.CountryName
                }).Where(U => U.WebsiteId == loggedInUserWebsiteId).GroupBy(Address => Address.CountryName).Select(group => new
                {
                    name = group.Key,
                    value = group.Count(),
                }).OrderBy(x => x.name).ToList());
            // add new flow in the database
            Flow flow = new Flow()
            {
                UserId = user.UserId
            };
            try
            {
                _flowRepository.AddFlow(flow);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _userRepository.DeleteUser(user.UserId);
                _websiteRepository.DeleteWebsite(websiteId);
                await Task.CompletedTask;
                return;
            }
            int flowId = flow.FlowId;
            // add new action in the database
            Models.Action action = new Models.Action()
            {
                Type = "Page Load",
                Content = url,
                FlowId = flowId,
                Page = url
            };
            try
            {
                _actionRepository.AddAction(action);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _flowRepository.DeleteFlow(flow.FlowId);
                _userRepository.DeleteUser(user.UserId);
                _websiteRepository.DeleteWebsite(websiteId);
                await Task.CompletedTask;
                return;
            }
            allactions = _actionRepository.GetAllActions();
            allflows = _flowRepository.GetAllFlows();
            allad = _userRepository.GetAddresses();
            allusers = _userRepository.GetAllUsers();
            int actioncount = allactions
                .Join(allflows, p => p.FlowId, u => u.FlowId, (p, u) => new { p, u })
                .Join(allusers, pu => pu.u.UserId, c => c.UserId, (pu, c) => new { pu.p, pu.u, c })
                .Where(puc => puc.c.WebsiteId == loggedInUserWebsiteId).Count();
            await Clients.All.SendAsync("ActionCount", actioncount);
            //var Action = _actionRepository.GetAllActions()
            //    .Join(_flowRepository.GetAllFlows(), p => p.FlowId, u => u.FlowId, (p, u) => new { p, u })
            //    .Join(_userRepository.GetAllUsers(), pu => pu.u.UserId, c => c.UserId, (pu, c) => new { pu.p, pu.u, c })
            //    .Where(puc => puc.c.WebsiteId == loggedInUserWebsiteId).Select(puc => new { puc.p.Page, puc.p.FlowId, puc.p.Type, puc.p.Content }).ToList();
            //List<string> uniqpage = Action.Select(u => u.Page).Distinct().ToList();
            //var Flow = _flowDataRepository.GetAllFlowDatas()
            //   .Join(_flowRepository.GetAllFlows(), p => p.FlowId, u => u.FlowId, (p, u) => new { p, u })
            //   .Join(_userRepository.GetAllUsers(), pu => pu.u.UserId, c => c.UserId, (pu, c) => new { pu.p, pu.u, c })
            //   .Where(puc => puc.c.WebsiteId == loggedInUserWebsiteId).Select(puc => new { puc.p.Page, puc.p.FlowId }).ToList();
            
            List<ActionSummary> Actions = _summaryData.GetAllActions(loggedInUserWebsiteId);
            foreach (var action_ in Actions)
            {
                string finalFlow = "";
                for (int i = 0; i < action_.ActionSummed.Length; i++)
                {
                    if (action_.ActionSummed[i] == '>')
                    {
                        finalFlow += "➜";
                    }
                    else if (action_.ActionSummed[i] == '/' && (action_.ActionSummed[i + 1] == '[' || action_.ActionSummed[i + 1] == ')'))
                    {
                        finalFlow += "LandingPage";
                    }
                    else
                    {
                        finalFlow += action_.ActionSummed[i];
                    }
                }
                action_.ActionSummed = finalFlow;

            }
            await Clients.All.SendAsync("ActionSummaryTable", Actions);
            FlowData flowdata = new FlowData()
            {
                FlowId = flowId,
                Page = url
            };

            try
            {
                _flowDataRepository.AddFlowData(flowdata);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _flowRepository.DeleteFlow(flow.FlowId);
                _userRepository.DeleteUser(user.UserId);
                _websiteRepository.DeleteWebsite(websiteId);
                await Task.CompletedTask;
                return;
            }
            List<FlowSummary> flows = _summaryData.GetAllFlows(loggedInUserWebsiteId);
            foreach (var f in flows)
            {
                string finalFlow = "";
                for (int i = 0; i < f.FlowSummed.Length; i++)
                {
                    if (f.FlowSummed[i] == '>')
                    {
                        finalFlow += "➜";
                    }
                    else if (i == f.FlowSummed.Length - 1 && f.FlowSummed[i] == '/')
                    {
                        finalFlow += "LandingPage";
                    }
                    else if (f.FlowSummed[i] == '/' && f.FlowSummed[i + 1] == '>')
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
            await Clients.All.SendAsync("SummaryTable", flows);
            string functionName = "AddNewUser";
            string userCookie = "webtracker_user";
            string userIdValue = user.UserId.ToString();
            string webCookie = "webtracker_web" + web;
            string webIdValue = website.WebsiteId.ToString();
            string flowSession = "webtracker_flow" + web;
            string flowIdValue = flow.FlowId.ToString();

            Console.WriteLine("Calling " + functionName + " passing data: ");
            Console.WriteLine(userCookie + " : " + userIdValue);
            Console.WriteLine(webCookie + " : " + webIdValue);
            Console.WriteLine(flowSession + " : " + flowIdValue);
            await Clients.Caller.SendAsync(functionName, userCookie, userIdValue, webCookie, webIdValue, flowSession, flowIdValue);
        }
        public async Task ExistingUser(string websiteId, string userId, string url)
        {
            await Clients.All.SendAsync("GetWebsiteId");
            Console.WriteLine("--- website id in existing user %%%%%%%%%%%%%%%%%%%%% " + loggedInUserWebsiteId);

            Console.WriteLine("Website with id = " + websiteId + " user id = " + userId + " to url : " + url);
            // update visit count of the website
            Website website;
            try
            {
                website = await _websiteRepository.GetWebsiteById(Convert.ToInt32(websiteId));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await Task.CompletedTask;
                return;
            }
            website.VisitCount++;
            try
            {
                _websiteRepository.UpdateWebsite(Convert.ToInt32(websiteId), website);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await Task.CompletedTask;
                return;
            }
            User user;
            try
            {
                user = _userRepository.GetUserById(Convert.ToInt32(userId));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await Task.CompletedTask;
                return;
            }
            user.ReturningData = DateTime.Now;
            try
            {
                _userRepository.UpdateUser(Convert.ToInt32(userId), user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await Task.CompletedTask;
                return;
            }
            int totalvisitors = _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId).Count();
            await Clients.All.SendAsync("TotalVisitors", totalvisitors);
            int retvisitors = _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId && u.ReturningData.Year != 0001).Count();
            await Clients.All.SendAsync("ReturningVisitors", retvisitors);
            int newvisitors = _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId && u.ReturningData.Year == 0001).Count();
            await Clients.All.SendAsync("NewVisitors", newvisitors);
            // create new flow
            Flow flow = new Flow()
            {
                UserId = Convert.ToInt32(userId)
            };
            try
            {
                _flowRepository.AddFlow(flow);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await Task.CompletedTask;
                return;
            }
            int flowId = flow.FlowId;
            // create new action
            Models.Action action = new Models.Action()
            {
                Type = "Page Load",
                Content = url,
                Page = url,
                FlowId = flowId
            };
            try
            {
                _actionRepository.AddAction(action);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _flowRepository.DeleteFlow(flow.FlowId);
                await Task.CompletedTask;
                return;
            }
            var allactions = _actionRepository.GetAllActions();
            var allflows = _flowRepository.GetAllFlows();
            var allad = _userRepository.GetAddresses();
            var allusers = _userRepository.GetAllUsers();
            int actioncount = allactions
                .Join(allflows, p => p.FlowId, u => u.FlowId, (p, u) => new { p, u })
                .Join(allusers, pu => pu.u.UserId, c => c.UserId, (pu, c) => new { pu.p, pu.u, c })
                .Where(puc => puc.c.WebsiteId == loggedInUserWebsiteId).Count();
            await Clients.All.SendAsync("ActionCount", actioncount);
            List<ActionSummary> Actions = _summaryData.GetAllActions(loggedInUserWebsiteId);
            foreach (var action_ in Actions)
            {
                string finalFlow = "";
                for (int i = 0; i < action_.ActionSummed.Length; i++)
                {
                    if (action_.ActionSummed[i] == '>')
                    {
                        finalFlow += "➜";
                    }
                    else if (action_.ActionSummed[i] == '/' && (action_.ActionSummed[i + 1] == '[' || action_.ActionSummed[i + 1] == ')'))
                    {
                        finalFlow += "LandingPage";
                    }
                    else
                    {
                        finalFlow += action_.ActionSummed[i];
                    }
                }
                action_.ActionSummed = finalFlow;

            }
            await Clients.All.SendAsync("ActionSummaryTable", Actions);
            FlowData flowdata = new FlowData()
            {
                Page = url,
                FlowId = flowId
            };
            try
            {
                _flowDataRepository.AddFlowData(flowdata);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _flowRepository.DeleteFlow(flow.FlowId);
                await Task.CompletedTask;
                return;
            }
            Console.WriteLine("Existing User Connected to " + website.Web);
            List<FlowSummary> flows = _summaryData.GetAllFlows(loggedInUserWebsiteId);
            foreach (var f in flows)
            {
                string finalFlow = "";
                for (int i = 0; i < f.FlowSummed.Length; i++)
                {
                    if (f.FlowSummed[i] == '>')
                    {
                        finalFlow += "➜";
                    }
                    else if (i == f.FlowSummed.Length - 1 && f.FlowSummed[i] == '/')
                    {
                        finalFlow += "LandingPage";
                    }
                    else if (f.FlowSummed[i] == '/' && f.FlowSummed[i + 1] == '>')
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
            await Clients.All.SendAsync("SummaryTable", flows);
            string functionName = "ExistingUser";
            string flowSession = "webtracker_flow" + website.Web;
            string flowIdValue = flow.FlowId.ToString();

            Console.WriteLine("Calling " + functionName + " passing data: ");
            Console.WriteLine(flowSession + " : " + flowIdValue);
            await Clients.Caller.SendAsync(functionName, flowSession, flowIdValue);
        }
        public async Task ExistingFlow(string flowId, string url)
        {
            await Clients.Caller.SendAsync("GetWebsiteId");
            Console.WriteLine("--- website id inexisting flow %%%%%%%%%%%%%%%%%%%%% " + loggedInUserWebsiteId);

            Console.WriteLine("User continue to the flow with id = " + flowId + " to url : " + url);
            // create new url

            // create new action
            Models.Action action = new Models.Action()
            {
                Type = "Page Load",
                Content = url,
                Page = url,
                FlowId = Convert.ToInt32(flowId)
            };
            try
            {
                _actionRepository.AddAction(action);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await Task.CompletedTask;
                return;
            }
            var allactions = _actionRepository.GetAllActions();
            var allflows = _flowRepository.GetAllFlows();
            var allad = _userRepository.GetAddresses();
            var allusers = _userRepository.GetAllUsers();
            int actioncount = allactions
                .Join(allflows, p => p.FlowId, u => u.FlowId, (p, u) => new { p, u })
                .Join(allusers, pu => pu.u.UserId, c => c.UserId, (pu, c) => new { pu.p, pu.u, c })
                .Where(puc => puc.c.WebsiteId == loggedInUserWebsiteId).Count();
            await Clients.All.SendAsync("ActionCount", actioncount);
            List<ActionSummary> Actions = _summaryData.GetAllActions(loggedInUserWebsiteId);
            foreach (var action_ in Actions)
            {
                string finalFlow = "";
                for (int i = 0; i < action_.ActionSummed.Length; i++)
                {
                    if (action_.ActionSummed[i] == '>')
                    {
                        finalFlow += "➜";
                    }
                    else if (action_.ActionSummed[i] == '/' && (action_.ActionSummed[i + 1] == '[' || action_.ActionSummed[i + 1] == ')'))
                    {
                        finalFlow += "LandingPage";
                    }
                    else
                    {
                        finalFlow += action_.ActionSummed[i];
                    }
                }
                action_.ActionSummed = finalFlow;

            }
            await Clients.All.SendAsync("ActionSummaryTable", Actions);
            FlowData flowdata = new FlowData()
            {
                Page = url,
                FlowId = Convert.ToInt32(flowId)
            };
            try
            {
                _flowDataRepository.AddFlowData(flowdata);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await Task.CompletedTask;
                return;
            }
            List<FlowSummary> flows = _summaryData.GetAllFlows(loggedInUserWebsiteId);
            foreach (var f in flows)
            {
                string finalFlow = "";
                for (int i = 0; i < f.FlowSummed.Length; i++)
                {
                    if (f.FlowSummed[i] == '>')
                    {
                        finalFlow += "➜";
                    }
                    else if (i == f.FlowSummed.Length - 1 && f.FlowSummed[i] == '/')
                    {
                        finalFlow += "LandingPage";
                    }
                    else if (f.FlowSummed[i] == '/' && f.FlowSummed[i + 1] == '>')
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
            await Clients.All.SendAsync("SummaryTable", flows);
        }
        public async Task ReceiveAction(string action, string data, string url, string flowid)
        {
            await Clients.Caller.SendAsync("GetWebsiteId");
            Console.WriteLine("--- website id in receive action %%%%%%%%%%%%%%%%%%%%% " + loggedInUserWebsiteId);

            Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
            Console.WriteLine("url: " + url);
            Console.WriteLine("action: " + action);
            Console.WriteLine("data: " + data);
            Console.WriteLine("flowid: " + flowid);
            Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");

            Models.Action actionObj = new Models.Action()
            {
                Type = action,
                Content = data,
                Page = url,
                FlowId = Convert.ToInt32(flowid)
            };
           
            _actionRepository.AddAction(actionObj);
            var allactions = _actionRepository.GetAllActions();
            var allflows = _flowRepository.GetAllFlows();
            var allad = _userRepository.GetAddresses();
            var allusers = _userRepository.GetAllUsers();
            int actioncount = allactions
                .Join(allflows, p => p.FlowId, u => u.FlowId, (p, u) => new { p, u })
                .Join(allusers, pu => pu.u.UserId, c => c.UserId, (pu, c) => new { pu.p, pu.u, c })
                .Where(puc => puc.c.WebsiteId == loggedInUserWebsiteId).Count();
            await Clients.All.SendAsync("ActionCount", actioncount);
            Console.WriteLine("Checking here~~~~~~~~~~~~~~" + loggedInUserWebsiteId); 
            List<ActionSummary> Actions = _summaryData.GetAllActions(loggedInUserWebsiteId);

            foreach (var action_ in Actions)
            {
                string finalFlow = "";
                for (int i = 0; i < action_.ActionSummed.Length; i++)
                {
                    if (action_.ActionSummed[i] == '>')
                    {
                        finalFlow += "➜";
                    }
                    else if (action_.ActionSummed[i] == '/' && (action_.ActionSummed[i + 1] == '[' || action_.ActionSummed[i + 1] == ')'))
                    {
                        finalFlow += "LandingPage";
                    }
                    else
                    {
                        finalFlow += action_.ActionSummed[i];
                    }
                }
                action_.ActionSummed = finalFlow;

            }

            await Clients.All.SendAsync("ActionSummaryTable", Actions);
            // add new action data in the database
            Console.WriteLine(Actions);
            Console.WriteLine("Action Performed: " + action);
            Console.WriteLine("Action Data: " + data);
        }

    }
}
