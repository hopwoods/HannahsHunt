using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HannahsHunt.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //Concatenate the First and Last names into fullname
        [Display(Name = "Full Name")]
        public string FullName { get
            {
                string dspFirstName =
                    string.IsNullOrWhiteSpace(this.FirstName) ? "" : this.FirstName;
                string dspLastName =
                    string.IsNullOrWhiteSpace(this.LastName) ? "" : this.LastName;

                return string
                    .Format("{0} {1}", dspFirstName, dspLastName);
            }
        }
    }
}
