using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class AppDbContext : IdentityUserContext<User, Guid>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
