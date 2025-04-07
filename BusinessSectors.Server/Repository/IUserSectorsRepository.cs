using BusinessSectors.Server.Data.Models;

namespace BusinessSectors.Server.Repository;

public interface IUserSectorsRepository
{
    Task<UserSectors?> GetByNameAsync(string name);
    Task<UserSectors?> GetByIdAsync(int id);
    Task<UserSectors> CreateAsync(string name, string? sectorsIds);
    Task UpdateSectorsAsync(int id, string sectorsIds);
    Task<bool> NameExistsAsync(string name);
}