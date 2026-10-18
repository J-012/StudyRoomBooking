using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Services;

namespace StudyRoomBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(
        IRoomService roomService,
        ILogger<RoomsController> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoomDto>>> GetAll()
    {
        List<RoomDto> rooms = await _roomService.GetAllAsync();

        return Ok(rooms);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDto>> GetById(int id)
    {
        RoomDto? room = await _roomService.GetByIdAsync(id);

        if (room == null)
        {
            _logger.LogWarning(
                "Room with id {RoomId} was not found.",
                id
            );

            return NotFound();
        }

        return Ok(room);
    }

    [HttpPost]
    public async Task<ActionResult<RoomDto>> Create(
        CreateRoomDto roomDto)
    {
        RoomDto createdRoom =
            await _roomService.CreateAsync(roomDto);

        _logger.LogInformation(
            "Room {RoomId} was created.",
            createdRoom.Id
        );

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdRoom.Id },
            createdRoom
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateRoomDto roomDto)
    {
        bool updated =
            await _roomService.UpdateAsync(id, roomDto);

        if (!updated)
        {
            _logger.LogWarning(
                "Room with id {RoomId} could not be updated because it was not found.",
                id
            );

            return NotFound();
        }

        _logger.LogInformation(
            "Room {RoomId} was updated.",
            id
        );

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted =
            await _roomService.DeleteAsync(id);

        if (!deleted)
        {
            _logger.LogWarning(
                "Room with id {RoomId} could not be deleted because it was not found.",
                id
            );

            return NotFound();
        }

        _logger.LogInformation(
            "Room {RoomId} was deleted.",
            id
        );

        return NoContent();
    }
}