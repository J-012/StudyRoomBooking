using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Services;

namespace StudyRoomBooking.Api.Controllers;

// Only admins can change amenities.
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class AmenitiesController : ControllerBase
{
    private readonly IAmenityService _amenityService;
    private readonly ILogger<AmenitiesController> _logger;

    public AmenitiesController(IAmenityService amenityService, ILogger<AmenitiesController> logger)
    {
        _amenityService = amenityService;
        _logger = logger;
    }
    // GET amenities
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<AmenityDto>>> GetAll() =>
        Ok(await _amenityService.GetAllAsync());

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<AmenityDto>> GetById(int id)
    {
        AmenityDto? amenity = await _amenityService.GetByIdAsync(id);

        if (amenity == null)
        {
            _logger.LogWarning("Amenity {AmenityId} was not found.", id);
            return NotFound();
        }

        return Ok(amenity);
    }
    // POST 
    [HttpPost]
    public async Task<ActionResult<AmenityDto>> Create(AmenityRequestDto amenityDto)
    {
        AmenityDto amenity = await _amenityService.CreateAsync(amenityDto);

        _logger.LogInformation("Amenity {AmenityId} was created.", amenity.Id);

        return CreatedAtAction(nameof(GetById), new { id = amenity.Id }, amenity);
    }
    // PUT
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AmenityRequestDto amenityDto)
    {
        if (!await _amenityService.UpdateAsync(id, amenityDto))
        {
            _logger.LogWarning("Amenity {AmenityId} could not be updated: not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Amenity {AmenityId} was updated.", id);

        return NoContent();
    }
    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _amenityService.DeleteAsync(id))
        {
            _logger.LogWarning("Amenity {AmenityId} could not be deleted: not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Amenity {AmenityId} was deleted.", id);

        return NoContent();
    }
}
