using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class AppDbContext : IdentityDbContext<User>
{
    public DbSet<Game> Games { get; set; } = default!;
    public AppDbContext(DbContextOptions options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder
            .Entity<Game>()
            .Property(e => e.Status)
            .HasConversion(
                v => v.ToString(),
                v => (Status)Enum.Parse(typeof(Status), v));
        modelBuilder
            .Entity<Game>()
            .HasKey(x => x.GameId);
        
        modelBuilder
            .Entity<IdentityUser<string>>()
            .HasKey(x => x.Id);
        modelBuilder
            .Entity<IdentityUser<string>>()
            .Property(x => x.Id);
    }
}