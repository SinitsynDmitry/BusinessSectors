using System.Reflection.Metadata;
using BusinessSectors.Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessSectors.Server.Data;

public class SectorsDbContext : DbContext
{
    public SectorsDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Sector> Sectors { get; set; }

    public DbSet<UserSectors> UserSectors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sector>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Order);

            entity.Property(s => s.Name).IsRequired().HasMaxLength(255);

            entity.Property(s => s.Path).IsRequired().HasMaxLength(1000); // Adjust based on max expected depth

            // Index for faster path-based queries
            entity.HasIndex(s => s.Path);
        });

        modelBuilder.Entity<UserSectors>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Name).IsRequired().HasMaxLength(255);

            entity.Property(s => s.SectorsIds).HasMaxLength(1000); // Adjust based on max expected depth

            entity.HasIndex(s => s.Name);
        });

    }
}
