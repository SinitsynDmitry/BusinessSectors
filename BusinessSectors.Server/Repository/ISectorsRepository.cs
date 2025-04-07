using BusinessSectors.Server.Data.Models;
using BusinessSectors.Server.DTOs;

namespace BusinessSectors.Server.Repository;
public interface ISectorsRepository
{
    ValueTask<IEnumerable<Sector>> GetSectorsAsync(int? userId);
    ValueTask UpdateSectorsAsync(IEnumerable<Sector> sectors);
}