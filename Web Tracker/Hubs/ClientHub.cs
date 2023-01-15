using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.SignalR;
using System;
using WebTracker.Models;
using WebTracker.Repositories;
using Newtonsoft.Json.Linq;
using WebTracker.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace WebTracker.Hubs
{
    public class ClientHub : Hub
    {
        IWebsiteRepository _websiteRepository;
        IUserRepository _userRepository;
        IFlowRepository _flowRepository;
        IActionRepository _actionRepository;
        IFlowDataRepository _flowDataRepository;
        public ClientHub(IWebsiteRepository websiteRepository, IUserRepository userRepository, IFlowRepository flowRepository, IActionRepository actionRepository, IFlowDataRepository flowDataRepository)
        {
            _websiteRepository = websiteRepository;
            _userRepository = userRepository;
            _flowRepository = flowRepository;
            _actionRepository = actionRepository;
            _flowDataRepository = flowDataRepository;
        }
        public async Task SendMessage(string user, object message)
        {
            Console.WriteLine(message);
        }
        public async Task AddNewUser(string web, string url, string deviceType, string browser, string os, string location, string OS)
        {
            Console.WriteLine("New User Connected to " + web + " at " + url + " with " + browser + " using " + deviceType + " from " + location);
            // check if website is in the database
            int websiteId = await _websiteRepository.GetWebsiteIdByName(web);

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
            FlowData flowdata = new FlowData()
            {
                FlowId = flowId,
                Page = url
            };

            try
            {
                _flowDataRepository.AddFlowData(flowdata
                    );
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
            user.ReturningData=DateTime.Now;
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
                Page=url,
                FlowId=flowId
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
            FlowData flowdata = new FlowData()
            {
                Page = url,
                FlowId = flowId
            };
            try
            {
                _flowDataRepository .AddFlowData(flowdata);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _flowRepository.DeleteFlow(flow.FlowId);
                await Task.CompletedTask;
                return;
            }
            Console.WriteLine("Existing User Connected to " + website.Web);

            string functionName = "ExistingUser";
            string flowSession = "webtracker_flow" + website.Web;
            string flowIdValue = flow.FlowId.ToString();
            
            Console.WriteLine("Calling " + functionName + " passing data: ");
            Console.WriteLine(flowSession + " : " + flowIdValue);
            await Clients.Caller.SendAsync(functionName, flowSession, flowIdValue);
        }
        public async Task ExistingFlow(string flowId, string url)
        {
            Console.WriteLine("User continue to the flow with id = " + flowId + " to url : " + url);
            // create new url
            
            // create new action
            Models.Action action = new Models.Action()
            {
                Type = "Page Load",
                Content = url,
                Page=url,
                FlowId= Convert.ToInt32(flowId)
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
        }
        public void ReceiveAction(string url, string action, string data, string flowid)
        {
            Models.Action actionObj = new Models.Action()
            {
                Type = action,
                Content = data,
                Page = url,
                FlowId = Convert.ToInt32(flowid)
            };
            _actionRepository.AddAction(actionObj);

            // add new action data in the database
            Console.WriteLine("Action Performed: " + action);
            Console.WriteLine("Action Data: " + data);
        }

    }
}
