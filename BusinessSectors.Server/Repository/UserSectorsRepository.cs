using BusinessSectors.Server.Data;
using BusinessSectors.Server.Data.Models;
using BusinessSectors.Server.Repository;
using Microsoft.EntityFrameworkCore;

public class UserSectorsRepository : IUserSectorsRepository
{
    private readonly SectorsDbContext _context;

    public UserSectorsRepository(SectorsDbContext context)
    {
        _context = context;
    }

    public async Task<UserSectors?> GetByNameAsync(string name)
    {
        var exist= await _context.UserSectors
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Name == name);
        if (exist != null)
        {
            return exist;
        }
        return await CreateAsync(name, "");
    }

    public async Task<UserSectors?> GetByIdAsync(int id)
    {
        return await _context.UserSectors.FindAsync(id);
    }

    public async Task<UserSectors> CreateAsync(string name, string? sectorsIds)
    {
        var userSectors = new UserSectors
        {
            Name = name,
            SectorsIds = sectorsIds
        };

        _context.UserSectors.Add(userSectors);
        await _context.SaveChangesAsync();
        return userSectors;
    }

    public async Task UpdateSectorsAsync(int id, string sectorsIds)
    {
        var userSectors = await _context.UserSectors.FindAsync(id);
        if (userSectors != null)
        {
            userSectors.SectorsIds = sectorsIds;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _context.UserSectors
            .AnyAsync(u => u.Name == name);
    }
}