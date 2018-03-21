using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HannahsHunt.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public object Roles { get; internal set; }

        internal void AddToRoleAsync(ApplicationUser user, string v)
        {
            throw new NotImplementedException();
        }
    }
}
