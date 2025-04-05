using System.ComponentModel.DataAnnotations;

namespace BusinessSectors.Server.DTOs;

public class SectorDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    public string Path { get; set; }  // e.g. "1/6/342"

    public bool IsSelected { get; set; }
}
