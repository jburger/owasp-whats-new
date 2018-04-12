using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using vulnerable.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace vulnerable.Controllers 
{
    public class AccountController : Controller 
    {
        private readonly IOptions<ApplicationSettings> settings;
        private readonly List<User> users;
        private readonly ILogger<AccountController> logger;

        public AccountController(IOptions<ApplicationSettings> settings, ILogger<AccountController> logger)
        {
            this.settings = settings;
            this.users = settings.Value.Users;
            this.logger = logger;
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            logger.LogInformation($"Successful logout for {User.Identity.Name}");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null, bool invalidUser = false) {
            TempData["returnUrl"] = returnUrl;
            ViewBag.invalidUser = invalidUser;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user, string returnUrl)
        {
            //todo: rate limiting
            //todo: weak password checks (e.g. passwords API from HBIP)
            if (user == null)
            {
                logger.LogDebug("No user details were posted, redirecting to /Account/Login");
                return RedirectToAction("Login", "Account", new { invalidUser=true });
            }
            var matchedUser = users.FirstOrDefault(u => u.Username == user.Username || u.MobilePhone == user.Username);

            logger.LogInformation($"Login attempt for {user.Username}");
            if(matchedUser == null) 
            {
                logger.LogInformation($"Couldn't find {user.Username}");
                return RedirectToAction("Login", "Account", new { invalidUser=true });
            }

            if (matchedUser?.Password != user.Password)
            {
                logger.LogInformation($"Invalid password attempt for user {user.Username}");
                return RedirectToAction("Login", "Account", new { invalidUser=true });
            }

            ClaimsIdentity identity = EstablishIdentity(matchedUser);

            await HttpContext
                .SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));
            logger.LogInformation($"Successful login for: {user.Username} ");
            return ReturnToUrl(returnUrl);
        }

        [Authorize(Policy="RequireAuthenticatedUser")]
        [HttpGet]
        public IActionResult Profile() {
            ViewBag.Username = User.Identity.Name;
            ViewBag.Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            ViewBag.MobilePhone = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value;
            ViewBag.Role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return View();
        }

        [Authorize(Policy="RequireAuthenticatedUser")]
        [HttpPost]
        public IActionResult Profile(User user) {
            return Ok(); //not necessary for the purposes of the demonstration      
        }

        
        private static ClaimsIdentity EstablishIdentity(User user)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(ClaimTypes.MobilePhone, user.MobilePhone));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Type == UserType.Admin ? "Administrator" : "User" ));
            return identity;
        }

        private IActionResult ReturnToUrl(string returnUrl)
        {
            if (returnUrl == null)
            {
                returnUrl = TempData["returnUrl"]?.ToString();
            }

            if (returnUrl != null)
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}