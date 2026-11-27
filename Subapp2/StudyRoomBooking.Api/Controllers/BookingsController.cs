using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Services;

namespace StudyRoomBooking.Api.Controllers;

// Broken domain rules (ArgumentException) and attempts to change someone
// else's booking (UnauthorizedAccessException) are turned into 400 and 403
// responses by GlobalExceptionHandler.
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        IBookingService bookingService,
        ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookingDto>>> GetAll()
    {
        string? userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        List<BookingDto> bookings =
            await _bookingService.GetAllAsync(
                userId,
                User.IsInRole("Admin")
            );

        return Ok(bookings);
    }

    // GET /api/bookings/open
    // All upcoming sessions, so logged-in students can find one to join.
    [HttpGet("open")]
    public async Task<ActionResult<List<BookingDto>>> GetOpen()
    {
        string? userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        List<BookingDto> bookings =
            await _bookingService.GetOpenAsync(
                userId,
                User.IsInRole("Admin")
            );

        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetById(int id)
    {
        string? userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        BookingDto? booking =
            await _bookingService.GetByIdAsync(
                id,
                userId,
                User.IsInRole("Admin")
            );

        if (booking == null)
        {
            _logger.LogWarning(
                "Booking with id {BookingId} was not found for user {UserId}.",
                id,
                userId
            );

            return NotFound();
        }

        return Ok(booking);
    }

    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create(
        CreateBookingDto bookingDto)
    {
        string? userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        BookingDto createdBooking =
            await _bookingService.CreateAsync(
                bookingDto,
                userId
            );

        _logger.LogInformation(
            "Booking {BookingId} created by user {UserId}.",
            createdBooking.Id,
            userId
        );

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdBooking.Id },
            createdBooking
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateBookingDto bookingDto)
    {
        string? userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        bool updated =
            await _bookingService.UpdateAsync(
                id,
                bookingDto,
                userId,
                User.IsInRole("Admin")
            );

        if (!updated)
        {
            _logger.LogWarning(
                "Booking with id {BookingId} could not be updated because it was not found.",
                id
            );

            return NotFound();
        }

        _logger.LogInformation(
            "Booking {BookingId} updated by user {UserId}.",
            id,
            userId
        );

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        string? userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        bool deleted =
            await _bookingService.DeleteAsync(
                id,
                userId,
                User.IsInRole("Admin")
            );

        if (!deleted)
        {
            _logger.LogWarning(
                "Booking with id {BookingId} could not be deleted because it was not found.",
                id
            );

            return NotFound();
        }

        _logger.LogInformation(
            "Booking {BookingId} deleted by user {UserId}.",
            id,
            userId
        );

        return NoContent();
    }

    // POST /api/bookings/5/participants
    // The current user joins the session.
    [HttpPost("{id}/participants")]
    public async Task<IActionResult> Join(int id)
    {
        string? userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        bool joined =
            await _bookingService.JoinAsync(id, userId);

        if (!joined)
        {
            _logger.LogWarning(
                "User {UserId} could not join booking {BookingId} because it was not found.",
                userId,
                id
            );

            return NotFound();
        }

        _logger.LogInformation(
            "User {UserId} joined booking {BookingId}.",
            userId,
            id
        );

        return NoContent();
    }

    // DELETE /api/bookings/5/participants
    // The current user leaves the session.
    [HttpDelete("{id}/participants")]
    public async Task<IActionResult> Leave(int id)
    {
        string? userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        bool left =
            await _bookingService.LeaveAsync(id, userId);

        if (!left)
        {
            _logger.LogWarning(
                "User {UserId} could not leave booking {BookingId} because it was not found.",
                userId,
                id
            );

            return NotFound();
        }

        _logger.LogInformation(
            "User {UserId} left booking {BookingId}.",
            userId,
            id
        );

        return NoContent();
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
