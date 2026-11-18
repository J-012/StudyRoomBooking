using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Services;

namespace StudyRoomBooking.Api.Controllers;

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
    public async Task<ActionResult<AmenityDto>> GetById(
        int id)
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
        try
        {
            AmenityDto amenity =
                await _amenityService.CreateAsync(
                    amenityDto
                );

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
        UpdateAmenityDto amenityDto)
    {
        try
        {
            bool updated =
                await _amenityService.UpdateAsync(
                    id,
                    amenityDto
                );

            if (!updated)
            {
                return NotFound();
            }

            _logger.LogInformation(
                "Amenity {AmenityId} was updated.",
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
            await _amenityService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        _logger.LogInformation(
            "Amenity {AmenityId} was deleted.",
            id
        );

        return NoContent();
    }
}