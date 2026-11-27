using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Services;

namespace StudyRoomBooking.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<RoomDto>>> GetAll() =>
        Ok(await _roomService.GetAllAsync());

    // GET all availabke rooms for a given time range
    [HttpGet("available")]
    [AllowAnonymous]
    public async Task<ActionResult<List<RoomDto>>> GetAvailable(
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime,
        [FromQuery] int? excludeBookingId) =>
        Ok(await _roomService.GetAvailableAsync(startTime, endTime, excludeBookingId));

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<RoomDto>> GetById(int id)
    {
        RoomDto? room = await _roomService.GetByIdAsync(id);

        if (room == null)
        {
            _logger.LogWarning("Room {RoomId} was not found.", id);
            return NotFound();
        }

        return Ok(room);
    }
    // POST rooms
    [HttpPost]
    public async Task<ActionResult<RoomDto>> Create(RoomRequestDto roomDto)
    {
        RoomDto room = await _roomService.CreateAsync(roomDto);

        _logger.LogInformation("Room {RoomId} was created.", room.Id);

        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }
    // PUT rooms
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, RoomRequestDto roomDto)
    {
        if (!await _roomService.UpdateAsync(id, roomDto))
        {
            _logger.LogWarning("Room {RoomId} could not be updated: not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Room {RoomId} was updated.", id);

        return NoContent();
    }
    // DELETE rooms
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _roomService.DeleteAsync(id))
        {
            _logger.LogWarning("Room {RoomId} could not be deleted: not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Room {RoomId} was deleted.", id);

        return NoContent();
    }
}
