using API.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; }

    public DbSet<Chat> Chats { get; set; }

    public DbSet<Match> Matches { get; set; }

    public DbSet<AuthToken> AuthTokens { get; set; }

    public DbSet<Group> Groups { get; set; }

    public DbSet<GroupChat> GroupChats { get; set; }

    public DbSet<GroupUser> GroupUsers { get; set; }

    public DbSet<GroupeAcces> GroupeAcces { get; set; }

    public DbSet<Feed> Feeds {get; set;}

    public DbSet<FeedComment> FeedComments {get; set;}

    public DbSet<FeedFiles> FeedFiles {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToCollection("users")
            .HasMany(b => b.Match)
            .WithOne(p => p.MatchedUser)
            .HasForeignKey(p => p.Id)
            .OnDelete(DeleteBehavior.NoAction);
    }
}