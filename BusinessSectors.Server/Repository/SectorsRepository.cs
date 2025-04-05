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

        // Return all sectors if no user specified
        if (string.IsNullOrWhiteSpace(userName))
        {
            return await sectorsQuery.ToListAsync();
        }

        // Execute both queries in parallel
        var userSectorsIdsTask = _context.UserSectors
            .AsNoTracking()
            .Where(u => u.Name == userName)
            .Select(u => u.SectorsIds)
            .FirstOrDefaultAsync();

        var sectorsTask = sectorsQuery.ToListAsync();

        await Task.WhenAll(userSectorsIdsTask, sectorsTask);

        var sectorsIdsString = await userSectorsIdsTask;
        var sectors = await sectorsTask;


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
