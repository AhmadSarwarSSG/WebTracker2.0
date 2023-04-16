using Microsoft.AspNetCore.Mvc;
using WebTracker.Models;
using System.Diagnostics;
using WebTracker.Repositories;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace WebTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IWebsiteRepository _websiteRepository;
        IUserRepository _userRepository;
        IFlowRepository _flowRepository;
        IActionRepository _actionRepository;
        IFlowDataRepository _flowDataRepository;
        public HomeController(ILogger<HomeController> logger, IWebsiteRepository websiteRepository, IUserRepository userRepository, IFlowRepository flowRepository, IActionRepository actionRepository, IFlowDataRepository flowDataRepository)
        {
            _logger = logger;
            _websiteRepository = websiteRepository;
            _userRepository = userRepository;
            _flowRepository = flowRepository;
            _actionRepository = actionRepository;
            _flowDataRepository = flowDataRepository;
        }

        public IActionResult Index()
        {
            if(HttpContext.Request.Cookies["username"] !=null)
            {
                ViewBag.userinfo = HttpContext.Request.Cookies["username"];

                int loggedInUserWebsiteId = int.Parse(HttpContext.Request.Cookies["loggedInUserWebsiteId"]);
                Console.WriteLine("------&&&&&&& loggedInUserWebsiteId: " + loggedInUserWebsiteId);

                ViewBag.BInfo = _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId).GroupBy(u => u.Browser).Select(group => new
                {
                    country = group.Key,
                    value = group.Count(),
                }).OrderBy(x => x.country).ToList();
                ViewBag.DInfo = _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId).GroupBy(u => u.DeviceType).Select(group => new
                {
                    name = group.Key,
                    value = group.Count(),
                }).OrderBy(x => x.name).ToList();
                ViewBag.OInfo = _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId).GroupBy(u => u.OS).Select(group => new
                {
                    country = group.Key,
                    value = group.Count(),
                }).OrderBy(x => x.country).ToList();
                ViewBag.DAInfo = _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId).GroupBy(u => u.LastConnection.ToString("yyyy-MM-dd")).Select(group => new
                {
                    date = group.Key,
                    value = group.Count(),
                }).OrderBy(x => x.date).ToList();
                ViewBag.AllUsers = _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId).Count();
                ViewBag.NewUsers = _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId && u.ReturningData.Year == 0001).Count();
                ViewBag.RetUsers = _userRepository.GetAllUsers().Where(u => u.WebsiteId == loggedInUserWebsiteId && u.ReturningData.Year != 0001).Count();
                var allactions = _actionRepository.GetAllActions();
                var allflows = _flowRepository.GetAllFlows();
                var allad = _userRepository.GetAddresses();
                var allusers = _userRepository.GetAllUsers();
                ViewBag.AInfo = allusers.Join(allad,
                    a => a.Address.AddressId,
                    b => b.AddressId,
                    (a, b) => new {
                        a.WebsiteId,
                        b.CountryName
                    }).Where(U => U.WebsiteId == loggedInUserWebsiteId).GroupBy(Address => Address.CountryName).Select(group => new
                    {
                        name = group.Key,
                        value = group.Count(),
                    }).OrderBy(x => x.name).ToList();
                ViewBag.actionsCount = allactions
                    .Join(allflows, p => p.FlowId, u => u.FlowId, (p, u) => new { p, u })
                    .Join(allusers, pu => pu.u.UserId, c => c.UserId, (pu, c) => new { pu.p, pu.u, c })
                    .Where(puc => puc.c.WebsiteId == loggedInUserWebsiteId).Count();

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

    }
}