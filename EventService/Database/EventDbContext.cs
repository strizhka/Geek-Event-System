using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace EventService.Data
{
    public class EventDbContext : DbContext
    {
        public EventDbContext(DbContextOptions<EventDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Participation> Participations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Event>()
                .Property(e => e.Description)
                .HasMaxLength(200);

            modelBuilder.Entity<Event>()
                .Property(e => e.Date)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.Address)
                .IsRequired();

            modelBuilder.Entity<Participation>()
                .Property(p => p.IsConfirmed)
                .HasDefaultValue(false);

            modelBuilder.Entity<Participation>()
                .HasIndex(p => new { p.UserId, p.EventId })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}

