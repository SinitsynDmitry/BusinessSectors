using BusinessSectors.Server.Data.DTOs;
using BusinessSectors.Server.Data.Models;

namespace BusinessSectors.Server.DTOs;

/// <summary>
/// The mapping.
/// </summary>
public static class Mapping
{
    /// <summary>
    /// To dto.
    /// </summary>
    /// <param name="userSectors">The user sectors.</param>
    /// <returns>An UserSectorsDto.</returns>
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

    /// <summary>
    /// To the entity.
    /// </summary>
    /// <param name="dto">The dto.</param>
    /// <returns>An UserSectors.</returns>
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

    /// <summary>
    /// Tos the dto.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>A SectorDto.</returns>
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

    /// <summary>
    /// Tos the entity.
    /// </summary>
    /// <param name="dto">The dto.</param>
    /// <returns>A Sector.</returns>
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
