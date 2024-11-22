using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace NotificationService.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>()
                .Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Notification>()
                .Property(n => n.Description)
                .HasMaxLength(200);

            base.OnModelCreating(modelBuilder);
        }
    }
}

