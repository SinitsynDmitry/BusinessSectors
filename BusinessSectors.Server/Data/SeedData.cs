using BusinessSectors.Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessSectors.Server.Data;

public static class SeedData
{
    public static void Initialize(SectorsDbContext context, IConfiguration configuration)
    {
        try
        {
            if (context.Sectors.Any())
                return; // Already seeded

            var sectorsConfig = configuration.GetSection("Sectors").Get<List<Sector>>() ?? new List<Sector>();

            context.Sectors.AddRange(sectorsConfig);
            context.SaveChanges();
            context.Database.ExecuteSqlRaw(
       "SELECT setval(pg_get_serial_sequence('\"Sectors\"', 'Id'), (SELECT MAX(\"Id\") FROM \"Sectors\"))");
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}
