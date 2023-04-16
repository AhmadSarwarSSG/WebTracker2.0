using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebTracker.Models;
using SignalR_Check.ViewModels;
using System.ComponentModel.DataAnnotations;
using WebTracker.Repositories;
using WebTracker.Hubs;

namespace SignalR_Check.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        IWebsiteRepository _websiteRepository;


        public AccountController(UserManager<ApplicationUser> uManager,
                                 SignInManager<ApplicationUser> sManager,
                                 IWebsiteRepository websiteRepository)
        {
            userManager = uManager;
            signInManager = sManager;

            this._websiteRepository = websiteRepository;
        }


        /*public IActionResult Index()
        {
            return View();
        }*/
        [HttpGet]
        public IActionResult Register()
        {
            RegisterViewModel vm = new RegisterViewModel();
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    WebsiteUrl = model.WebsiteUrl
                };
                
                var result = await userManager.CreateAsync(user,
                                                         model.Password);

                if (result.Succeeded)
                {
                    
                    if (signInManager.IsSignedIn(User))
                    {
                        return RedirectToAction("index", "home");
                    }
                    return RedirectToAction("login", "account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }

        //--------------login---------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await
                signInManager.PasswordSignInAsync(model.Username,
                                              model.Password, false, false);
                if (result.Succeeded)
                {
                    //save its login credentials
                    var loggedinUser = await userManager.FindByNameAsync(model.Username);
                    var loggedinUserWebsiteUrl = loggedinUser.WebsiteUrl;
                    Console.WriteLine("url " + loggedinUserWebsiteUrl);
                    int loggedInUserWebsiteId = _websiteRepository.GetWebsiteIdByName(loggedinUserWebsiteUrl);
            Console.WriteLine("------&&&&&&& loggedInUserWebsiteId controller: " + loggedInUserWebsiteId);

                    //making cookie of this wesbite Id
                    CookieOptions options = new CookieOptions();
                    options.Expires = DateTime.Now.AddDays(1);
                    HttpContext.Response.Cookies.Append("loggedInUserWebsiteId", loggedInUserWebsiteId.ToString(), options);
                    HttpContext.Response.Cookies.Append("username", model.Username, options);

                    //send this cookie in clienHub class
                    ClientHub.loggedInUserWebsiteId = loggedInUserWebsiteId;

                    return RedirectToAction("index", "home");
                }
                ModelState.AddModelError(string.Empty, "Invalid Username or Password\n");

            }
            return View(model);
        }

        //----logout---------------
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            HttpContext.Response.Cookies.Delete("username");
            return RedirectToAction("login", "Account");
        }



    }
}
