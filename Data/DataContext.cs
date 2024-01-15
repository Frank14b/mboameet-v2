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
    }
}