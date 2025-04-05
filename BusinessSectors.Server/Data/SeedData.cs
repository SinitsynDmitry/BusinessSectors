using BusinessSectors.Server.Data.Models;

namespace BusinessSectors.Server.Data;

public static class SeedData
{
    public static void Initialize(SectorsDbContext context, IConfiguration configuration)
    {
        if (context.Sectors.Any())
            return; // Already seeded

        var sectorsConfig = configuration.GetSection("Sectors").Get<List<Sector>>();
        context.Sectors.AddRange(sectorsConfig);
        context.SaveChanges();
    }
}
