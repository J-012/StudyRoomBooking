using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Services;

namespace StudyRoomBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class BuildingsController : ControllerBase
{
    private readonly IBuildingService _buildingService;
    private readonly ILogger<BuildingsController> _logger;

    public BuildingsController(IBuildingService buildingService, ILogger<BuildingsController> logger)
    {
        _buildingService = buildingService;
        _logger = logger;
    }
    // GET buildings
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<BuildingDto>>> GetAll() =>
        Ok(await _buildingService.GetAllAsync());

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<BuildingDto>> GetById(int id)
    {
        BuildingDto? building = await _buildingService.GetByIdAsync(id);

        if (building == null)
        {
            _logger.LogWarning("Building {BuildingId} was not found.", id);
            return NotFound();
        }

        return Ok(building);
    }
    // POST buildings
    [HttpPost]
    public async Task<ActionResult<BuildingDto>> Create(BuildingRequestDto buildingDto)
    {
        BuildingDto building = await _buildingService.CreateAsync(buildingDto);

        _logger.LogInformation("Building {BuildingId} was created.", building.Id);

        return CreatedAtAction(nameof(GetById), new { id = building.Id }, building);
    }
    // PUT buildings
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BuildingRequestDto buildingDto)
    {
        if (!await _buildingService.UpdateAsync(id, buildingDto))
        {
            _logger.LogWarning("Building {BuildingId} could not be updated: not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Building {BuildingId} was updated.", id);

        return NoContent();
    }
    // DELETE buildings
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _buildingService.DeleteAsync(id))
        {
            _logger.LogWarning("Building {BuildingId} could not be deleted: not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Building {BuildingId} was deleted.", id);

        return NoContent();
    }
}
