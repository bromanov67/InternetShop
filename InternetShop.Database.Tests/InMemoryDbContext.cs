using InternetShop.Database.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetShop.Database.Tests
{
    public class InMemoryDbContext : UserDbContext
    {
        public InMemoryDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }
    }
}
