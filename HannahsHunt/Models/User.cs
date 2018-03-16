using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HannahsHunt.Models
{
    public class User
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAdddress { get; set; }
    }
}