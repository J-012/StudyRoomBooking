using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Services;

namespace StudyRoomBooking.Api.Controllers;

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

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetById(
        int id)
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

        try
        {
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
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
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

        try
        {
            bool updated =
                await _bookingService.UpdateAsync(
                    id,
                    bookingDto,
                    userId,
                    User.IsInRole("Admin")
                );

            if (!updated)
            {
                return NotFound();
            }

            _logger.LogInformation(
                "Booking {BookingId} updated by user {UserId}.",
                id,
                userId
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
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        string? userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        try
        {
            bool deleted =
                await _bookingService.DeleteAsync(
                    id,
                    userId,
                    User.IsInRole("Admin")
                );

            if (!deleted)
            {
                return NotFound();
            }

            _logger.LogInformation(
                "Booking {BookingId} deleted by user {UserId}.",
                id,
                userId
            );

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirstValue(
                   JwtRegisteredClaimNames.Sub)
               ??
               User.FindFirstValue(
                   ClaimTypes.NameIdentifier);
    }
}