using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace CommunityService.Data
{
    public class CommunityDbContext : DbContext
    {
        public CommunityDbContext(DbContextOptions<CommunityDbContext> options) : base(options) { }

        public DbSet<Community> Communities { get; set; }
        public DbSet<Membership> Memberships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Community>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Community>()
                .Property(c => c.Description)
                .HasMaxLength(200);

            modelBuilder.Entity<Membership>()
                .Property(m => m.IsConfirmed)
                .HasDefaultValue(false);

            modelBuilder.Entity<Membership>()
                .HasIndex(m => new { m.UserId, m.CommunityId })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}




