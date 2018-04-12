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
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Text;
using vulnerable.Domain;

namespace vulnerable.Controllers 
{
    public class AccountController : Controller 
    {
        private readonly IOptions<ApplicationSettings> settings;
        private readonly string secret;
        private readonly List<User> users;
        private readonly string COOKIE_KEY = "PiggyBankCo";

        public AccountController(IOptions<ApplicationSettings> settings)
        {
            this.settings = settings;
            this.users = settings.Value.Users;
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete(COOKIE_KEY);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null, bool invalidPass = false, bool invalidUser = false) {
            TempData["returnUrl"] = returnUrl;
            ViewBag.invalidUser = invalidUser;
            ViewBag.invalidPass = invalidPass;
            return View();
        }

        [HttpGet]
        public IActionResult Profile() {
            var user = Utils.GetIdentity(Request);

            if(user == null) 
                return RedirectToAction("Index", "Home");
            ViewBag.Username = user.Username;
            ViewBag.Email = user.Email;
            ViewBag.MobilePhone = user.MobilePhone;
            ViewBag.Role = user.Type == UserType.Admin ? "Administrator" : "User";

            return View();
        }

        [HttpPost]
        public IActionResult Profile(User user) {
            return Ok(); //not necessary for the purposes of the demonstration      
        }

        [HttpPost]
        public IActionResult Login(User user, string returnUrl)
        {
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { invalidUser=true });
            }
            var matchedUser = users.FirstOrDefault(u => u.Username == user.Username || u.MobilePhone == user.Username);

            if(matchedUser == null) 
            {
                return RedirectToAction("Login", "Account", new { invalidUser=true });
            }

            if (matchedUser?.Password != user.Password)
            {
                return RedirectToAction("Login", "Account", new { invalidPass=true });
            }

            EstablishIdentity(matchedUser, Response);
            return ReturnToUrl(returnUrl);
        }

        private void EstablishIdentity(User user, HttpResponse response)
        {
            string cookie = CreateCookie(user);
            response.Cookies.Append(COOKIE_KEY, cookie, new CookieOptions
            {
                Path = "/",
                HttpOnly = false
            });
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

        private static string CreateCookie(User user)
        {
            //strip out the sensitive data. GDPR is coming.
            user.Password = "";
            user.MobilePhone = "";
            user.Email = "";

            var json = JsonConvert.SerializeObject(user);
            var buffer = Encoding.UTF8.GetBytes(json);
            var cookie = Convert.ToBase64String(buffer);
            return cookie;
        }
    }
}