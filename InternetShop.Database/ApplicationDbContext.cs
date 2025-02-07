using InternetShop.Database.Configurations;
using InternetShop.Database.Models;
using InternetShop.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InternetShop.Database
{
    public class ApplicationDbContext : IdentityDbContext<User> //swap from DbContext to Identity DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Вызов базового метода.

            // Здесь применяем настройки для вашего пользовательского класса UserEntity
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            // Настройка первичного ключа для IdentityUserLogin
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
            });
        }

    }
}