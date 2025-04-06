using BusinessSectors.Server.Data;
using BusinessSectors.Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessSectors.Server.Repository;

public class SectorsRepository : ISectorsRepository
{
    private readonly SectorsDbContext _context;

    public SectorsRepository(SectorsDbContext context)
    {
        _context = context;
    }

    public async ValueTask<IEnumerable<Sector>> GetSectorsAsync(string? userName)
    {

        var sectorsQuery = _context.Sectors.AsNoTracking();
        var sectors = await sectorsQuery.ToListAsync();

        // Return all sectors if no user specified
        if (string.IsNullOrWhiteSpace(userName))
        {
            return sectors;
        }

        // Execute both queries in parallel
        var sectorsIdsString = await _context.UserSectors
            .AsNoTracking()
            .Where(u => u.Name == userName)
            .Select(u => u.SectorsIds)
            .FirstOrDefaultAsync();

        

        if (string.IsNullOrWhiteSpace(sectorsIdsString))
        {
            return sectors;
        }


        var userSectorIds = sectorsIdsString
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(idStr => int.TryParse(idStr, out var id) ? id : -1)
            .Where(id => id > 0)
            .ToHashSet();


        if (userSectorIds.Count > 0)
        {
            foreach (var sector in sectors)
            {
                sector.IsSelected = userSectorIds.Contains(sector.Id);
            }
        }

        return sectors;
    }
}
