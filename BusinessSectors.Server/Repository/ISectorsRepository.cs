using BusinessSectors.Server.Data.Models;

namespace BusinessSectors.Server.Repository;
public interface ISectorsRepository
{
    ValueTask<IEnumerable<Sector>> GetSectorsAsync(string? userName);
}