using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HannahsHunt.Data;
using HannahsHunt.Models;
using HannahsHunt.Models.UsersViewModels;
using HannahsHunt.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections;
using HannahsHunt.Extensions;

namespace HannahsHunt.Controllers
{
    [Authorize(Roles="Administrator")]
    public class UsersController : Controller
    {
        #region Initilisation and Context
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        //Create DB Context
        private readonly ApplicationDbContext _context;

        //Initialise Context
        public UsersController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<UsersController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }      

        //Clean Up Context
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        #endregion

        #region Index

        /// <summary>
        /// Fetch and display a lis of Users and their associated role(s)
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        // GET: Users/
        public async Task<IActionResult> Index(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            List<UsersWithRolesViewModel> usersWithRoles = new List<UsersWithRolesViewModel>();
            List<ApplicationUser> users = _userManager.Users.ToList();
            foreach (var user in users)
            {// Get the roles for the user
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new UsersWithRolesViewModel()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles
                });
            }
            _logger.LogInformation("Users list presented.");
            return View(usersWithRoles);
        }
        #endregion

        #region User Details

        /// <summary>
        /// Fetch detailsof a specific User and pass to the Details view
        /// </summary>
        /// <param name="id"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        // GET: Users/Details/5
        public async Task<ActionResult> Details(string id, UsersDetailsViewModel userDetails, string returnUrl = null)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);

            userDetails.Id = user.Id;
            userDetails.FirstName = user.FirstName;
            userDetails.LastName = user.LastName;
            userDetails.FullName = user.FullName;
            userDetails.UserName = user.UserName;
            userDetails.Email = user.Email;
            userDetails.PhoneNumber = user.PhoneNumber;
            userDetails.Roles = roles.ToList();
            ViewData["ReturnUrl"] = returnUrl;
            return View(userDetails);
        }
        #endregion

        #region Create Users
        /// <summary>
        /// Display the Create User View
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        // GET: Users/Create
        [HttpGet]
        public IActionResult Create(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Create a new Identity User based on the provided values. Create role, and claims for the new user.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);


                    //Add Claims!
                    await _userManager.AddClaimAsync(user, new Claim("FirstName", user.FirstName));
                    await _userManager.AddClaimAsync(user, new Claim("LastName", user.LastName));
                    await _userManager.AddClaimAsync(user, new Claim("FullName", user.FullName));

                    if (model.IsAdmin == true)
                    {
                        await _userManager.AddToRoleAsync(user, "Administrator");
                    } else
                    {
                        await _userManager.AddToRoleAsync(user, "Basic");
                    }                    
                    _logger.LogInformation("User created a new account with password.");

                    ViewBag.Message = "<strong>Success!</strong> The User has been created";
                    return View();
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
#endregion

        #region Edit Users

        /// <summary>
        /// Fetch Details of a specific Identity User and return to Edit View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Users/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            IList<string> role = await _userManager.GetRolesAsync(user);
            bool isAdmin = false;
            foreach(string item in role)
            {
                if (item == "Administrator")
                {
                    isAdmin = true;
                }
            }

            var model = new EditUserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsAdmin = isAdmin,
                Id = user.Id,
                StatusMessage = null                
            };
            return View(model);
        }

        /// <summary>
        /// Save the edited details of a User Identity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditConfirmed(EditUserViewModel model)
        {
            if (model.Id == null)
            {
                return NotFound();
            }
            ApplicationUser userToUpdate = await _userManager.FindByIdAsync(model.Id);            
            try
            {
                if (ModelState.IsValid)
                {
                    userToUpdate.FirstName = model.FirstName;
                    userToUpdate.LastName = model.LastName;
                    userToUpdate.Email = model.Email;
                    userToUpdate.PhoneNumber = model.PhoneNumber;                   

                    var result = await _userManager.UpdateAsync(userToUpdate);
                    if(result.Succeeded)
                    {
                        if(model.IsAdmin == true)
                        {
                            await _userManager.RemoveFromRoleAsync(userToUpdate,"Basic");
                            await _userManager.AddToRoleAsync(userToUpdate,"Administrator");
                        } else
                        {
                            await _userManager.RemoveFromRoleAsync(userToUpdate, "Administrator");
                            await _userManager.AddToRoleAsync(userToUpdate, "Basic");
                        }

                        //Update Claims
                        // Located in Extentions.cs
                        await User.AddUpdateClaimAsync(_userManager, userToUpdate, "FirstName", userToUpdate.FirstName);
                        await User.AddUpdateClaimAsync(_userManager, userToUpdate, "LastName", userToUpdate.LastName);
                        await User.AddUpdateClaimAsync(_userManager, userToUpdate, "FullName", userToUpdate.FullName);

                        ////Get the Users current claims
                        //var claims = await _userManager.GetClaimsAsync(userToUpdate);

                        //// Remove existing claim and replace with a new value

                        ////First Name
                        //if (claims.FirstOrDefault(c => c.Type == "FirstName") != null)
                        //{
                        //    await _userManager.RemoveClaimAsync(userToUpdate, claims.FirstOrDefault(c => c.Type == "FirstName"));
                        //    var setFirstNameResult = await _userManager.AddClaimAsync(userToUpdate, new Claim("FirstName", userToUpdate.FirstName));
                        //    if (!setFirstNameResult.Succeeded)
                        //    {
                        //        throw new ApplicationException($"Unexpected error occurred setting First Name for user with ID '{userToUpdate.Id}'.");
                        //    }
                        //}
                        //else
                        //{
                        //    var setFirstNameResult = await _userManager.AddClaimAsync(userToUpdate, new Claim("FirstName", userToUpdate.FirstName));
                        //    if (!setFirstNameResult.Succeeded)
                        //    {
                        //        throw new ApplicationException($"Unexpected error occurred setting First Name for user with ID '{userToUpdate.Id}'.");
                        //    }
                        //}

                        ////Last Name
                        //if (claims.FirstOrDefault(c => c.Type == "LastName") != null)
                        //{
                        //    await _userManager.RemoveClaimAsync(userToUpdate, claims.FirstOrDefault(c => c.Type == "LastName"));
                        //    var setLastNameResult = await _userManager.AddClaimAsync(userToUpdate, new Claim("LastName", userToUpdate.LastName));
                        //    if (!setLastNameResult.Succeeded)
                        //    {
                        //        throw new ApplicationException($"Unexpected error occurred setting Last Name for user with ID '{userToUpdate.Id}'.");
                        //    }
                        //}
                        //else
                        //{
                        //    var setLastNameResult = await _userManager.AddClaimAsync(userToUpdate, new Claim("LastName", userToUpdate.LastName));
                        //    if (!setLastNameResult.Succeeded)
                        //    {
                        //        throw new ApplicationException($"Unexpected error occurred setting Last Name for user with ID '{userToUpdate.Id}'.");
                        //    }
                        //}

                        ////Full Name
                        //if (claims.FirstOrDefault(c => c.Type == "FullName") != null)
                        //{
                        //    await _userManager.RemoveClaimAsync(userToUpdate, claims.FirstOrDefault(c => c.Type == "FullName"));
                        //    var setFullNameResult = await _userManager.AddClaimAsync(userToUpdate, new Claim("FullName", userToUpdate.FullName));
                        //    if (!setFullNameResult.Succeeded)
                        //    {
                        //        throw new ApplicationException($"Unexpected error occurred setting Full Name for user with ID '{userToUpdate.Id}'.");
                        //    }
                        //}
                        //else
                        //{
                        //    var setFullNameResult = await _userManager.AddClaimAsync(userToUpdate, new Claim("FullName", userToUpdate.FullName));
                        //    if (!setFullNameResult.Succeeded)
                        //    {
                        //        throw new ApplicationException($"Unexpected error occurred setting Full Name for user with ID '{userToUpdate.Id}'.");
                        //    }
                        //}
                        model.StatusMessage = "Success! User: " + model.FirstName + "...Details Saved";
                    }
                }
                return View("Edit",model);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating user {id}", model.Id);
                model.StatusMessage = "Error! Something went wrong please try again later";
                return View(model);
            }
        }
        #endregion

        #region Delete Users
        // GET: Users/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(string id, UsersWithRolesViewModel userWithRoles, string returnUrl = null)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);

            userWithRoles.Id = user.Id;
            userWithRoles.FirstName = user.FirstName;
            userWithRoles.LastName = user.LastName;
            userWithRoles.FullName = user.FullName;
            userWithRoles.UserName = user.UserName;
            userWithRoles.Email = user.Email;
            userWithRoles.PhoneNumber = user.PhoneNumber;
            userWithRoles.Roles = roles.ToList();
            ViewData["ReturnUrl"] = returnUrl;
            return View(userWithRoles);
        }
        
        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                //get User Data from Userid
                var user = await _userManager.FindByIdAsync(id);

                //List Logins associated with user
                IList<UserLoginInfo> logins = await _userManager.GetLoginsAsync(user);

                //Gets list of Roles associated with current user
                var rolesForUser = await _userManager.GetRolesAsync(user);

                using (var transaction = _context.Database.BeginTransaction())
                {
                    foreach (var login in logins)
                    {
                        await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                    }

                    if (rolesForUser.Count() > 0)
                    {
                        foreach (var item in rolesForUser.ToList())
                        {
                            // item should be the name of the role
                            var result = await _userManager.RemoveFromRoleAsync(user, item);
                        }
                    }
                    //Delete User
                    await _userManager.DeleteAsync(user);

                    _logger.LogInformation("User {0} deleted.", user.Id);

                    TempData["Message"] = "User Deleted Successfully. ";
                    TempData["MessageValue"] = "1";
                    transaction.Commit();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError("User Deletion Failed: {0}", ex.Message);

                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";

                return View();
            }
        }
        #endregion

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}