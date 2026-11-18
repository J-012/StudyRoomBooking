using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Services;

namespace StudyRoomBooking.Api.Controllers;

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
    public async Task<ActionResult<BuildingDto>> GetById(
        int id)
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
        try
        {
            BuildingDto building =
                await _buildingService.CreateAsync(
                    buildingDto
                );

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
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        UpdateBuildingDto buildingDto)
    {
        try
        {
            bool updated =
                await _buildingService.UpdateAsync(
                    id,
                    buildingDto
                );

            if (!updated)
            {
                return NotFound();
            }

            _logger.LogInformation(
                "Building {BuildingId} was updated.",
                id
            );

            return NoContent();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        int id)
    {
        bool deleted =
            await _buildingService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        _logger.LogInformation(
            "Building {BuildingId} was deleted.",
            id
        );

        return NoContent();
    }
}