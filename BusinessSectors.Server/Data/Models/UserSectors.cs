using System.ComponentModel.DataAnnotations;

namespace BusinessSectors.Server.Data.Models;

public class UserSectors
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [StringLength(1000)]
    public string? SectorsIds { get; set; }
}
