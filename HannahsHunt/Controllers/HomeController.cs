using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HannahsHunt.Models;
using Microsoft.AspNetCore.Authorization;

namespace HannahsHunt.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        #region Home Page
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region About Page
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        #endregion

        #region Contact Page
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        #endregion

        #region Errors
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}
