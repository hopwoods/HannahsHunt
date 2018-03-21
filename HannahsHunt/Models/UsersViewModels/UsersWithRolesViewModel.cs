using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HannahsHunt.Models.UsersViewModels
{
    public class UsersWithRolesViewModel
    {
        public string UserName { get; set; }
        [Display(Name = "User Email Address")]
        
        public string UserEmail { get; set; }
        public string PhoneNumber { get; set; }
        public IList<IdentityUserRole<int>> Roles { get; set; }
    }
}
