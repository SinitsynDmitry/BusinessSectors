using System.ComponentModel.DataAnnotations;

namespace BusinessSectors.Server.Data.DTOs;

public class UserSectorsDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    public List<int> SectorsIds { get; set; }
}
