using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HannahsHunt.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string UserID { get; set; }

        [Required]
        public string Password { get; set; }

        public string Salt { get; set; }

        [Required]
        [MaxLength(255), MinLength(2)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255), MinLength(1)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(255), MinLength(1)]
        public string EmailAdddress { get; set; }
    }
}