using HannahsHunt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HannahsHunt.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            var userid = principal.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            return userid?.Value;
        }
        public static string GetFirstName(this ClaimsPrincipal principal)
        {
            var firstname = principal.Claims.FirstOrDefault(c => c.Type == "FirstName");
            return firstname?.Value;
        }
        public static string GetLastName(this ClaimsPrincipal principal)
        {
            var lastname = principal.Claims.FirstOrDefault(c => c.Type == "LastName");
            return lastname?.Value;
        }
        public static string GetFullName(this ClaimsPrincipal principal)
        {
            var fullName = principal.Claims.FirstOrDefault(c => c.Type == "FullName");
            return fullName?.Value;
        }
        public static string GetUserClaim(this ClaimsPrincipal principal, string claimType)
        {
            var claim = principal.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }

        //Method for updating User Claims in the Database
        public static async Task AddUpdateClaimAsync(this ClaimsPrincipal principal, UserManager<ApplicationUser> userManager, ApplicationUser user, string claimType, string value)
        {
            UserManager<ApplicationUser> _userManager = userManager;

            //Get the Users current claims
            var claims = await userManager.GetClaimsAsync(user);

            // Check if claim exists, then...            
            if (claims.FirstOrDefault(c => c.Type == claimType) != null)
            {
                // Remove existing claim and replace with a new value
                await _userManager.RemoveClaimAsync(user, claims.FirstOrDefault(c => c.Type == claimType));
                var result = await _userManager.AddClaimAsync(user, new Claim(claimType, value));
                if (!result.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting Claim '{claimType}' for user with ID '{user.Id}'.");
                }
            }
            else
            {
                // Add Claim with value
                var result = await _userManager.AddClaimAsync(user, new Claim(claimType, value));
                if (!result.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting Claim '{claimType}' for user with ID '{user.Id}'.");
                }
            }
        }
    }
}

