using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HannahsHunt.Models
{
    public class HannahsHuntContext : DbContext
    {
        public HannahsHuntContext (DbContextOptions<HannahsHuntContext> options)
            : base(options)
        {
        }

        public DbSet<HannahsHunt.Models.User> User { get; set; }
    }
}
