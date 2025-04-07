using BusinessSectors.Server.Data.DTOs;
using BusinessSectors.Server.Data.Models;

namespace BusinessSectors.Server.DTOs;

public static class Mapping
{
    public static UserSectorsDto ToDto(this UserSectors userSectors)
    {
        return new UserSectorsDto
        {
            Id = userSectors.Id,
            Name = userSectors.Name,
            SectorsIds = userSectors.SectorsIds?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList() ?? new List<int>()
        };
    }

    public static UserSectors ToEntity(this UserSectorsDto dto)
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

    public static SectorDto ToDto(this Sector entity)
    {
        return new SectorDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Order = entity.Order,
            Path = entity.Path,
            IsSelected = entity.IsSelected
        };
    }

    public static Sector ToEntity(this SectorDto dto)
    {
        return new Sector
        {
            Id = dto.Id,
            Name = dto.Name,
            Order = dto.Order,
            Path = dto.Path,
            IsSelected = dto.IsSelected
        };
    }
}
