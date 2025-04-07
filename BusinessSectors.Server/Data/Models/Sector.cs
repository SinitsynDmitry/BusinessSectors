using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSectors.Server.Data.Models;

public class Sector
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    public string Path { get; set; }  // e.g. "/1/6/342/"

    public int Order { get; set; }

    [NotMapped]
    public bool IsSelected { get; set; } = false;

}
