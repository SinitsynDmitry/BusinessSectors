﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSectors.Server.Data.Models;

public class Sector
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
    /// Gets or sets the path.
    /// </summary>
    [Required]
    public string Path { get; set; }  // e.g. "/1/6/342/"

    /// <summary>
    /// Gets or sets the order.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether is selected.
    /// </summary>
    [NotMapped]
    public bool IsSelected { get; set; } = false;

}
