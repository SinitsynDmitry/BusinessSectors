using BusinessSectors.Server.Data.Models;
using BusinessSectors.Server.Repository;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("api/[controller]")]
public class UserSectorsController : ControllerBase
{
    private readonly IUserSectorsRepository _repository;

    public UserSectorsController(IUserSectorsRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<UserSectors>> GetUserSectors(string name)
    {
        var userSectors = await _repository.GetByNameAsync(name);
        return userSectors != null ? Ok(userSectors) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<UserSectors>> CreateUserSectors(
        [FromBody] CreateUserSectorsRequest request)
    {
        if (await _repository.NameExistsAsync(request.Name))
        {
            return Conflict("User with this name already exists");
        }

        var userSectors = await _repository.CreateAsync(request.Name, request.SectorsIds);
        return CreatedAtAction(nameof(GetUserSectors), new { name = userSectors.Name }, userSectors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserSectors(
        int id,
        [FromBody] UpdateUserSectorsRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        await _repository.UpdateSectorsAsync(id, request.SectorsIds);
        return NoContent();
    }
}

public class CreateUserSectorsRequest
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [StringLength(1000)]
    public string? SectorsIds { get; set; }
}

public class UpdateUserSectorsRequest
{
    [Required]
    [StringLength(1000)]
    public string SectorsIds { get; set; }
}