using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace CommunityService.Data
{
    public class CommunityDbContext : DbContext
    {
        public CommunityDbContext(DbContextOptions<CommunityDbContext> options) : base(options) { }

        public DbSet<Community> Communities { get; set; }
        public DbSet<Membership> Memberships { get; set; }
    }
}

