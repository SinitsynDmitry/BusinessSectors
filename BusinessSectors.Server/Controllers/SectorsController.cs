using BusinessSectors.Server.DTOs;
using BusinessSectors.Server.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BusinessSectors.Server.Controllers;

[Route("api/sectors")]
[ApiController]
public class SectorsController : ControllerBase
{
    private readonly ISectorsRepository _repository;

    public SectorsController(ISectorsRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SectorDto>>> GetSectors(int? userId)
    {
        var sectors = await _repository.GetSectorsAsync(userId);
        return Ok(sectors);
    }
}
