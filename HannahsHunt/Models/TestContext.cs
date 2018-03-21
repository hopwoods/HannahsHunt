using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HannahsHunt.Models
{
    public class TestContext
    {
       public int Id { get; set; }
       public string Test { get; set; }
       public virtual ApplicationUser User { get; set; }
    }
}
