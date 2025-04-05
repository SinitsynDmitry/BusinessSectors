using BusinessSectors.Server.Data.DTOs;
using BusinessSectors.Server.Data.Models;

namespace BusinessSectors.Server.DTOs;

public static class Mapping
{
    public static UserSectorsDTO ToDto(this UserSectors userSectors)
    {
        return new UserSectorsDTO
        {
            Id = userSectors.Id,
            Name = userSectors.Name,
            SectorsIds = userSectors.SectorsIds?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList() ?? new List<int>()
        };
    }

    public static UserSectors ToEntity(this UserSectorsDTO dto)
    {
        return new UserSectors
        {
            Id = dto.Id,
            Name = dto.Name,
            SectorsIds = dto.SectorsIds != null && dto.SectorsIds.Any()
                ? string.Join(",", dto.SectorsIds)
                : null
        };
    }
}
