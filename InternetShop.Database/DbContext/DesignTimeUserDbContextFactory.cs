using InternetShop.Database.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InternetShop.Database
{
    public class DesignTimeUserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=dbshop;Username=bogda;Password=1234");

            return new UserDbContext(optionsBuilder.Options);
        }
    }
}