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

    public async ValueTask UpdateSectorsAsync(IEnumerable<Sector> sectors)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var incomingSectors = sectors.ToList();
            var existingSectors = await _context.Sectors.AsNoTracking().ToListAsync();

            var sectorsToRemove = existingSectors
                .Where(s => !incomingSectors.Any(i => i.Id == s.Id))
                .ToList();
            if (sectorsToRemove.Count > 0)
                _context.Sectors.RemoveRange(sectorsToRemove);

            var idMapping = new Dictionary<int, int>();
            var addedSectors = new List<(Sector Original, Sector Entity)>();

            foreach (var incoming in incomingSectors.Where(s => s.Id == 0 || !existingSectors.Any(e => e.Id == s.Id)))
            {
                var entity = new Sector
                {
                    Id = -1, // New ID will be generated
                    Name = incoming.Name,
                    Path = incoming.Path,
                    Order = incoming.Order
                };

                await _context.Sectors.AddAsync(entity);
                await _context.SaveChangesAsync(); // Get new ID

                if (incoming.Id != 0)
                    idMapping[incoming.Id] = entity.Id;

                addedSectors.Add((incoming, entity));
            }

            // Update paths of children based on new ID mapping
            foreach (var (original, entity) in addedSectors)
            {
                if (!idMapping.TryGetValue(original.Id, out var newId))
                    continue;

                var oldSegment = $"/{original.Id}/";
                var newSegment = $"/{newId}/";

                foreach (var sector in _context.Sectors.Local.Where(s => s.Path.Contains(oldSegment)))
                    sector.Path = sector.Path.Replace(oldSegment, newSegment);
            }

            // Update existing
            foreach (var incoming in incomingSectors.Where(s => s.Id > 0))
            {
                var existing = await _context.Sectors.FindAsync(incoming.Id);
                if (existing == null)
                    continue;

                existing.Name = incoming.Name;
                existing.Path = incoming.Path;
                existing.Order = incoming.Order;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async ValueTask<IEnumerable<Sector>> GetSectorsAsync(int? userId)
    {

        var sectorsQuery = _context.Sectors.AsNoTracking();
        var sectors = await sectorsQuery.ToListAsync();

        // Return all sectors if no user specified
        if (userId is null || userId < 1)
        {
            return sectors;
        }

        // Execute both queries in parallel
        var sectorsIdsString = await _context.UserSectors
            .AsNoTracking()
            .Where(u => u.Id == userId)
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
