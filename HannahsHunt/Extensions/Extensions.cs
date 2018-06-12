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
    }
}

