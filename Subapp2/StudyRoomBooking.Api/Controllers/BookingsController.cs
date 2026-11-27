using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Services;

namespace StudyRoomBooking.Api.Controllers;

// All booking endpoints require a logged-in user student or admin
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    // The logged-in user's id from the JWT
    private string CurrentUserId =>
        User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("The token does not contain a user id.");

    private bool IsAdmin => User.IsInRole(Roles.Admin);

    // GET bookings
    [HttpGet]
    public async Task<ActionResult<List<BookingDto>>> GetAll() =>
        Ok(await _bookingService.GetAllAsync(CurrentUserId, IsAdmin));

    // GET bookings/open
    // All upcoming sessions, so logged-in students can find one to join.
    [HttpGet("open")]
    public async Task<ActionResult<List<BookingDto>>> GetOpen() =>
        Ok(await _bookingService.GetOpenAsync(CurrentUserId, IsAdmin));

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetById(int id)
    {
        BookingDto? booking = await _bookingService.GetByIdAsync(id, CurrentUserId, IsAdmin);

        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} was not found for user {UserId}.", id, CurrentUserId);
            return NotFound();
        }

        return Ok(booking);
    }
    // POST bookings
    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create(BookingRequestDto bookingDto)
    {
        BookingDto booking = await _bookingService.CreateAsync(bookingDto, CurrentUserId);

        _logger.LogInformation("Booking {BookingId} created by user {UserId}.", booking.Id, CurrentUserId);

        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }
    // PUT bookings
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BookingRequestDto bookingDto)
    {
        if (!await _bookingService.UpdateAsync(id, bookingDto, CurrentUserId, IsAdmin))
        {
            _logger.LogWarning("Booking {BookingId} could not be updated: not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Booking {BookingId} updated by user {UserId}.", id, CurrentUserId);

        return NoContent();
    }
    // DELETE bookings
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _bookingService.DeleteAsync(id, CurrentUserId, IsAdmin))
        {
            _logger.LogWarning("Booking {BookingId} could not be deleted: not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Booking {BookingId} deleted by user {UserId}.", id, CurrentUserId);

        return NoContent();
    }

    // POST bookings when the current user joins the session.
    [HttpPost("{id}/participants")]
    public async Task<IActionResult> Join(int id)
    {
        if (!await _bookingService.JoinAsync(id, CurrentUserId))
        {
            _logger.LogWarning("User {UserId} could not join booking {BookingId}: not found.", CurrentUserId, id);
            return NotFound();
        }

        _logger.LogInformation("User {UserId} joined booking {BookingId}.", CurrentUserId, id);

        return NoContent();
    }

    // The current user leaves the session.
    [HttpDelete("{id}/participants")]
    public async Task<IActionResult> Leave(int id)
    {
        if (!await _bookingService.LeaveAsync(id, CurrentUserId))
        {
            _logger.LogWarning("User {UserId} could not leave booking {BookingId}: not found.", CurrentUserId, id);
            return NotFound();
        }

        _logger.LogInformation("User {UserId} left booking {BookingId}.", CurrentUserId, id);

        return NoContent();
    }
}
