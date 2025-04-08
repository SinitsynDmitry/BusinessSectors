using System.ComponentModel.DataAnnotations;

namespace BusinessSectors.Server.Data.DTOs;

public class UserSectorsDto
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the sectors ids.
    /// </summary>
    public List<int> SectorsIds { get; set; }
}

public class CreateUserSectorsDto
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the sectors ids.
    /// </summary>
    [StringLength(1000)]
    public string? SectorsIds { get; set; }
}

public class UpdateUserSectorsDto
{
    /// <summary>
    /// Gets or sets the sectors ids.
    /// </summary>
    [StringLength(1000)]
    public string? SectorsIds { get; set; }
}
