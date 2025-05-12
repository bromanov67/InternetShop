using InternetShop.Database.Models;
using InternetShop.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InternetShop.Database
{
    public class UserDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}   