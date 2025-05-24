using InternetShop.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InternetShop.Database.DbContext
{
    public class UserDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
    {
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderItemEntity> OrderItems { get; set; }
        public DbSet<OrderStatusEntity> OrderStatuses { get; set; }
        public DbSet<PaymentEntity> Payments { get; set; }
        public DbSet<PaymentTypeEntity> PaymentTypes { get; set; }
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            const int statusPendingId = 1;
            const int statusProcessingId = 2;
            const int statusCompletedId = 3;
            const int statusCancelledId = 4;

            const int paymentCreditCardId = 1;
            const int paymentPayPalId = 2;
            const int paymentBankTransferId = 3;

            builder.Entity<OrderEntity>(b =>
            {
                b.HasKey(o => o.Id);
                b.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId);
                b.HasOne(o => o.Status)
                .WithMany()
                .HasForeignKey(o => o.StatusId);
            });

            builder.Entity<OrderItemEntity>(b =>
            {
                b.HasKey(oi => oi.Id);
                b.HasOne(oi => oi.Order)
                    .WithMany(o => o.Items)
                    .HasForeignKey(oi => oi.OrderId);
            });

            builder.Entity<OrderStatusEntity>(b =>
            {
                b.ToTable("OrderStatuses");
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            });

            // PaymentType
            builder.Entity<PaymentTypeEntity>(b =>
            {
                b.ToTable("PaymentTypes");
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).HasMaxLength(100).IsRequired();
                b.HasData(
                    new PaymentTypeEntity { Id = paymentCreditCardId, Name = "Credit Card", Description = "Payment by credit card" },
                    new PaymentTypeEntity { Id = paymentPayPalId, Name = "PayPal", Description = "Payment via PayPal" },
                    new PaymentTypeEntity { Id = paymentBankTransferId, Name = "Bank Transfer", Description = "Payment by bank transfer" }
                );
            });

            // Payment
            builder.Entity<PaymentEntity>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.TransactionId).HasMaxLength(100).IsRequired();
                b.Property(x => x.Status).HasMaxLength(50).IsRequired();
                b.HasOne(x => x.Order)
                    .WithOne(x => x.Payment)
                    .HasForeignKey<PaymentEntity>(x => x.OrderId);
                b.HasOne(p => p.PaymentType)
                .WithMany()
                .HasForeignKey(p => p.PaymentTypeId);
            });
        }
    }
}