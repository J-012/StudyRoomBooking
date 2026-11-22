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
public class AmenitiesController : ControllerBase
{
    private readonly IAmenityService _amenityService;
    private readonly ILogger<AmenitiesController> _logger;

    public AmenitiesController(
        IAmenityService amenityService,
        ILogger<AmenitiesController> logger)
    {
        _amenityService = amenityService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<AmenityDto>>> GetAll()
    {
        List<AmenityDto> amenities =
            await _amenityService.GetAllAsync();

        return Ok(amenities);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<AmenityDto>> GetById(int id)
    {
        AmenityDto? amenity =
            await _amenityService.GetByIdAsync(id);

        if (amenity == null)
        {
            _logger.LogWarning(
                "Amenity with id {AmenityId} was not found.",
                id
            );

            return NotFound();
        }

        return Ok(amenity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AmenityDto>> Create(
        CreateAmenityDto amenityDto)
    {
        AmenityDto amenity =
            await _amenityService.CreateAsync(amenityDto);

        _logger.LogInformation(
            "Amenity {AmenityId} was created.",
            amenity.Id
        );

        return CreatedAtAction(
            nameof(GetById),
            new { id = amenity.Id },
            amenity
        );
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        int id,
        UpdateAmenityDto amenityDto)
    {
        bool updated =
            await _amenityService.UpdateAsync(id, amenityDto);

        if (!updated)
        {
            _logger.LogWarning(
                "Amenity with id {AmenityId} could not be updated because it was not found.",
                id
            );

            return NotFound();
        }

        _logger.LogInformation(
            "Amenity {AmenityId} was updated.",
            id
        );

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted =
            await _amenityService.DeleteAsync(id);

        if (!deleted)
        {
            _logger.LogWarning(
                "Amenity with id {AmenityId} could not be deleted because it was not found.",
                id
            );

            return NotFound();
        }

        _logger.LogInformation(
            "Amenity {AmenityId} was deleted.",
            id
        );

        return NoContent();
    }
}
