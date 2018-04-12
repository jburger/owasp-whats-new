using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using vulnerable.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using vulnerable.Domain;

namespace vulnerable.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            if (!Utils.IsAdmin(Request)) {
                return NotFound();
            }
            return View();
        }
    }
}