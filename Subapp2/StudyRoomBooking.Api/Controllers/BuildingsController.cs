using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Services;

namespace StudyRoomBooking.Api.Controllers;

// Broken domain rules (ArgumentException from the service) are turned into
// 400 responses by GlobalExceptionHandler.
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BuildingsController : ControllerBase
{
    private readonly IBuildingService _buildingService;
    private readonly ILogger<BuildingsController> _logger;

    public BuildingsController(
        IBuildingService buildingService,
        ILogger<BuildingsController> logger)
    {
        _buildingService = buildingService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<BuildingDto>>> GetAll()
    {
        List<BuildingDto> buildings =
            await _buildingService.GetAllAsync();

        return Ok(buildings);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<BuildingDto>> GetById(int id)
    {
        BuildingDto? building =
            await _buildingService.GetByIdAsync(id);

        if (building == null)
        {
            _logger.LogWarning(
                "Building with id {BuildingId} was not found.",
                id
            );

            return NotFound();
        }

        return Ok(building);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<BuildingDto>> Create(
        CreateBuildingDto buildingDto)
    {
        BuildingDto building =
            await _buildingService.CreateAsync(buildingDto);

        _logger.LogInformation(
            "Building {BuildingId} was created.",
            building.Id
        );

        return CreatedAtAction(
            nameof(GetById),
            new { id = building.Id },
            building
        );
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        UpdateBuildingDto buildingDto)
    {
        bool updated =
            await _buildingService.UpdateAsync(id, buildingDto);

        if (!updated)
        {
            _logger.LogWarning(
                "Building with id {BuildingId} could not be updated because it was not found.",
                id
            );

            return NotFound();
        }

        _logger.LogInformation(
            "Building {BuildingId} was updated.",
            id
        );

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted =
            await _buildingService.DeleteAsync(id);

        if (!deleted)
        {
            _logger.LogWarning(
                "Building with id {BuildingId} could not be deleted because it was not found.",
                id
            );

            return NotFound();
        }

        _logger.LogInformation(
            "Building {BuildingId} was deleted.",
            id
        );

        return NoContent();
    }
}
