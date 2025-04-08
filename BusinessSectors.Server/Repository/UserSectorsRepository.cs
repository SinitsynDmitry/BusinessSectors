using BusinessSectors.Server.Data;
using BusinessSectors.Server.Data.Models;
using BusinessSectors.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

public class UserSectorsRepository : IUserSectorsRepository
{
    private readonly SectorsDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSectorsRepository"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public UserSectorsRepository(SectorsDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    /// <summary>
    /// Gets the by name async.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A ValueTask.</returns>
    public async ValueTask<UserSectors?> GetByNameAsync(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        return await _context.UserSectors
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Name == name)
            ?? await CreateAsync(name, string.Empty);
    }

    /// <summary>
    /// Gets the by id async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns>A ValueTask.</returns>
    public async ValueTask<UserSectors?> GetByIdAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);

        return await _context.UserSectors
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Creates the async.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="sectorsIds">The sectors ids.</param>
    /// <returns>A ValueTask.</returns>
    public async ValueTask<UserSectors> CreateAsync(string name, string? sectorsIds)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var userSectors = new UserSectors
        {
            Name = name.Trim(),
            SectorsIds = sectorsIds ?? string.Empty
        };

        _context.UserSectors.Add(userSectors);
        await _context.SaveChangesAsync();
        return userSectors;
    }

    /// <summary>
    /// Updates the sectors async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="sectorsIds">The sectors ids.</param>
    /// <returns>A ValueTask.</returns>
    public async ValueTask<bool> UpdateSectorsAsync(int id, string sectorsIds)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(id, 0);
        ArgumentNullException.ThrowIfNull(sectorsIds);

        var userSectors = await _context.UserSectors.FindAsync(id);
        if (userSectors is null)
            return false;

        userSectors.SectorsIds = sectorsIds;
        return await _context.SaveChangesAsync() > 0;

    }
}