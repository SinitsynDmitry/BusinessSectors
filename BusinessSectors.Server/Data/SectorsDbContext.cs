using BusinessSectors.Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessSectors.Server.Data;


/// <summary>
/// The sectors db context.
/// </summary>
public class SectorsDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SectorsDbContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public SectorsDbContext(DbContextOptions options)
        : base(options) { }

    /// <summary>
    /// Gets or sets the sectors.
    /// </summary>
    public DbSet<Sector> Sectors { get; set; }

    /// <summary>
    /// Gets or sets the user sectors.
    /// </summary>
    public DbSet<UserSectors> UserSectors { get; set; }
}
