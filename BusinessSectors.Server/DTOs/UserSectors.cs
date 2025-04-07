using System.ComponentModel.DataAnnotations;

namespace BusinessSectors.Server.Data.DTOs;

public class UserSectorsDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    public List<int> SectorsIds { get; set; }
}

public class CreateUserSectorsDto
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [StringLength(1000)]
    public string? SectorsIds { get; set; }
}

public class UpdateUserSectorsDto
{
    [StringLength(1000)]
    public string? SectorsIds { get; set; }
}
