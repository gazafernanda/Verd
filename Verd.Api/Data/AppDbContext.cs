using Microsoft.EntityFrameworkCore;
using Verd.Api.Models;

namespace Verd.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Plant> Plants => Set<Plant>();
    public DbSet<PlantLog> PlantLogs => Set<PlantLog>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Plant>()
            .HasOne(p => p.User)
            .WithMany(u => u.Plants)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SystemSetting>()
            .HasIndex(s => s.Key)
            .IsUnique();

        modelBuilder.Entity<PlantLog>()
            .HasOne(l => l.Plant)
            .WithMany(p => p.Logs)
            .HasForeignKey(l => l.PlantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
