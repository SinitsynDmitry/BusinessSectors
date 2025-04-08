using BusinessSectors.Server.Data;
using BusinessSectors.Server.Data.Models;
using BusinessSectors.Server.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BusinessSectors.Server.Repository;

public class SectorsRepository : ISectorsRepository
{
    private readonly SectorsDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SectorsRepository"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public SectorsRepository(SectorsDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    /// <summary>
    /// Updates the sectors async.
    /// </summary>
    /// <param name="sectors">The sectors.</param>
    /// <returns>A ValueTask.</returns>
    public async ValueTask<bool> UpdateSectorsAsync(IEnumerable<Sector> sectors)
    {
        ArgumentNullException.ThrowIfNull(sectors);

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var incomingSectors = sectors.ToList();
            var existingSectors = await _context.Sectors.AsNoTracking().ToListAsync();

            // Create lookup collections
            var existingIds = existingSectors.Select(s => s.Id).ToHashSet();
            var incomingIds = incomingSectors.Select(s => s.Id).Where(id => id > 0).ToHashSet();

            var sectorsToRemove = existingSectors.Where(s => !incomingIds.Contains(s.Id)).ToList();
            if (sectorsToRemove.Count > 0)
            {
                _context.Sectors.RemoveRange(sectorsToRemove);
            }

            // Process additions with temporary path storage
            var newSectors = incomingSectors
                .Where(s => s.Id == 0 || !existingIds.Contains(s.Id))
                .Select(s => new
                {
                    OriginalId = s.Id,
                    Entity = new Sector
                    {
                        Name = s.Name.Trim(),
                        Path = s.Path,
                        Order = s.Order
                    }
                })
                .ToList();

            // Bulk add new sectors
            foreach (var item in newSectors)
            {
                await _context.Sectors.AddAsync(item.Entity);
            }
            await _context.SaveChangesAsync();

            // Create ID mapping for path updates
            var idMapping = newSectors
                .Where(x => x.OriginalId != 0)
                .ToDictionary(x => x.OriginalId, x => x.Entity.Id);

            // Batch update paths
            foreach (var (oldId, newId) in idMapping)
            {
                var oldSegment = $"/{oldId}/";
                var newSegment = $"/{newId}/";

                var affectedSectors = _context.Sectors.Local
                    .Where(s => s.Path.Contains(oldSegment))
                    .ToList();

                foreach (var sector in affectedSectors)
                {
                    sector.Path = sector.Path.Replace(oldSegment, newSegment);
                }
            }

            // Process updates for existing sectors
            var sectorsToUpdate = incomingSectors
                .Where(s => s.Id > 0 && existingIds.Contains(s.Id))
                .ToList();

            foreach (var incoming in sectorsToUpdate)
            {
                var existing = existingSectors.First(s => s.Id == incoming.Id);
                existing.Name = incoming.Name;
                existing.Path = incoming.Path;
                existing.Order = incoming.Order;
                _context.Entry(existing).State = EntityState.Modified;
            }

            var changesSaved = await _context.SaveChangesAsync() > 0;
            await transaction.CommitAsync();

            return changesSaved || sectorsToRemove.Count > 0 || newSectors.Count > 0;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Gets the sectors async.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>A ValueTask.</returns>
    public async ValueTask<IEnumerable<Sector>> GetSectorsAsync(int? userId)
    {
        try
        {
            var sectors = await _context.Sectors.AsNoTracking().ToListAsync();

            if (userId is null or <= 0)
            {
                return sectors;
            }

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
        catch (Exception ex)
        {
            throw new ValidationException("Failed to retrieve sectors", ex);
        }
    }
}