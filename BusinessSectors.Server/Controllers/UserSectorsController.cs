using BusinessSectors.Server.Data.Models;
using BusinessSectors.Server.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserSectorsController : ControllerBase
{
    private readonly IUserSectorsRepository _repository;
    private readonly ILogger<UserSectorsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSectorsController"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="logger">The logger.</param>
    public UserSectorsController(
        IUserSectorsRepository repository,
        ILogger<UserSectorsController> logger)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(logger);
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Gets user sectors by name
    /// </summary>
    [HttpGet("{name}")]
    [ProducesResponseType(typeof(UserSectors), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserSectors>> GetUserSectors(string name)
    {
        var userSectors = await _repository.GetByNameAsync(name);
        return userSectors != null ? Ok(userSectors) : NotFound();
    }

    /// <summary>
    /// Updates user sectors
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ApiResponse>> UpdateUserSectors(
        [FromRoute, Range(1, int.MaxValue)] int id,
        [FromBody] UpdateUserSectorsRequest request)
    {
        _logger.LogInformation("Updating sectors for user {UserId}", id);

        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse(false, "Invalid request", ModelState));
        }

        try
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound(new ApiResponse(false, $"User with ID {id} not found"));
            }

            var success = await _repository.UpdateSectorsAsync(id, request.SectorsIds);
            return success ? NoContent() : BadRequest(new ApiResponse(false, "Update failed"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user sectors");
            return BadRequest(new ApiResponse(false, "An error occurred"));
        }
    }
}

public record ApiResponse(bool Success, string Message = "", object? Data = null);

public class UpdateUserSectorsRequest
{
    /// <summary>
    /// Gets or sets the sectors ids.
    /// </summary>
    [Required(ErrorMessage = "SectorsIds is required")]
    [RegularExpression(@"^(\d+,)*\d+$", ErrorMessage = "Comma-separated numbers expected")]
    public string SectorsIds { get; set; } = string.Empty;
}