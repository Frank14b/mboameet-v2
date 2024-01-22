using API.Entities;
using Microsoft.EntityFrameworkCore;
using API.Seeders;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<AppUser> Users { get; set; }

        public DbSet<AppChat> Chats { get; set; }

        public DbSet<AppMatch> Matches { get; set; }

        public DbSet<AppAuthToken> AuthTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AppUser>()
                .HasMany(b => b.Match)
                .WithOne(p => p.MatchedUser)
                .HasForeignKey(p => p.Id);
        }
    }
}