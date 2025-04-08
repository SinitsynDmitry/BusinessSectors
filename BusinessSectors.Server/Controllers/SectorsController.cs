using BusinessSectors.Server.DTOs;
using BusinessSectors.Server.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSectors.Server.Controllers;

[ApiController]
[Route("api/sectors")]
[Produces("application/json")]
public class SectorsController : ControllerBase
{
    private readonly ISectorsRepository _repository;
    private readonly ILogger<SectorsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SectorsController"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="logger">The logger.</param>
    public SectorsController(
        ISectorsRepository repository,
        ILogger<SectorsController> logger)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(logger);
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Gets all sectors with optional user-specific selection status
    /// </summary>
    /// <param name="userId">Optional user ID to check selected sectors</param>
    /// <response code="200">Returns list of sectors</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="500">If there was a server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<SectorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyCollection<SectorDto>>> GetSectors([FromQuery] int? userId)
    {
        try
        {
            _logger.LogInformation("Getting sectors for user {UserId}", userId);

            var sectors = await _repository.GetSectorsAsync(userId);
            return Ok(sectors.Select(s => s.ToDto()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sectors for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse(false, "An error occurred while retrieving sectors"));
        }
    }

    /// <summary>
    /// Updates the complete sector hierarchy
    /// </summary>
    /// <param name="sectors">Complete list of sectors with hierarchy information</param>
    /// <response code="204">Update successful</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="500">Server error during update</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateSectors([FromBody] IReadOnlyCollection<SectorDto> sectors)
    {
        _logger.LogInformation("Updating sector hierarchy");

        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse(false, "Invalid request data", ModelState));
        }

        try
        {
            var success = await _repository.UpdateSectorsAsync(
                sectors.Select(s => s.ToEntity()));

            return success
                ? NoContent()
                : BadRequest(new ApiResponse(false, "Sector update failed"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sectors");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse(false, "An error occurred while updating sectors"));
        }
    }
}