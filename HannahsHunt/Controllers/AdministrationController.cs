using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HannahsHunt.Models;
using HannahsHunt.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace HannahsHunt.Controllers
{
    [Authorize(Roles="Administrator")]
    public class AdministrationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        //Create DB Context
        private readonly ApplicationDbContext _context;

        //Initialise Context
        public AdministrationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Clean Up Context
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        

        public IActionResult Users()
        {
           List<ApplicationUser> users = _userManager.Users.ToList();
           return View(users);
        }
    }
}