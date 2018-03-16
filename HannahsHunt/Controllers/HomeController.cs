using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HannahsHunt.Models;

namespace HannahsHunt.Controllers
{
    public class HomeController : Controller
    {
        private readonly HannahsHuntContext _context;

        public HomeController(HannahsHuntContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var user = _context.Users.First();
            return View(user);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
