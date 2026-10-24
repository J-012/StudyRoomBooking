using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Models;
using StudyRoomBooking.Repositories;
using StudyRoomBooking.Services;

namespace StudyRoomBooking.Controllers;

[Authorize]
public class BookingsController : Controller
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingRepository bookingRepository, IRoomRepository roomRepository, IUserRepository userRepository, IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var bookings = await _bookingRepository.GetByUserIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        return View(bookings);
    }

    public async Task<IActionResult> Details(int id)
    {
        var booking = await GetBookingForCurrentUserAsync(id);
        return booking is null ? NotFound() : View(booking);
    }

    public async Task<IActionResult> Create()
    {
        var now = DateTime.Now;
        var startMinutes = (now.Hour * 60 + now.Minute + 14) / 15 * 15;
        var start = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(Math.Min(startMinutes, 23 * 60 + 45)));
        var end = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(Math.Min(start.Hour * 60 + start.Minute + 60, 23 * 60 + 45)));

        return View(new BookingCreateViewModel
        {
            Rooms = await _roomRepository.GetAllAsync(),
            StartDate = DateOnly.FromDateTime(now),
            StartClock = start,
            EndClock = end
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookingCreateViewModel viewModel)
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return await ReturnBookingFormAsync(viewModel);
        }

        var booking = CreateBooking(viewModel, user);
        AddValidationErrors(await _bookingService.ValidateAsync(booking, null));
        if (!ModelState.IsValid)
        {
            return await ReturnBookingFormAsync(viewModel);
        }

        await _bookingRepository.AddAsync(booking);
        _logger.LogInformation("Booking {BookingId} created for room {RoomId}.", booking.BookingId, booking.RoomId);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var booking = await GetBookingForCurrentUserAsync(id);
        if (booking is null)
        {
            return NotFound();
        }

        return View(new BookingCreateViewModel
        {
            BookingId = booking.BookingId,
            Subject = booking.Subject,
            Topic = booking.Topic,
            RoomId = booking.RoomId,
            Rooms = await _roomRepository.GetAllAsync(),
            StartDate = DateOnly.FromDateTime(booking.StartTime),
            StartClock = TimeOnly.FromDateTime(booking.StartTime),
            EndClock = TimeOnly.FromDateTime(booking.EndTime)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookingCreateViewModel viewModel)
    {
        var existingBooking = await GetBookingForCurrentUserAsync(id);
        if (existingBooking is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return await ReturnBookingFormAsync(viewModel);
        }

        var candidate = new Booking
        {
            BookingId = existingBooking.BookingId,
            UserId = existingBooking.UserId,
            UserName = existingBooking.UserName,
            Subject = viewModel.Subject,
            Topic = viewModel.Topic,
            RoomId = viewModel.RoomId,
            StartTime = viewModel.StartDate.ToDateTime(viewModel.StartClock),
            EndTime = viewModel.StartDate.ToDateTime(viewModel.EndClock)
        };

        AddValidationErrors(await _bookingService.ValidateAsync(candidate, id));
        if (!ModelState.IsValid)
        {
            return await ReturnBookingFormAsync(viewModel);
        }

        existingBooking.Subject = candidate.Subject;
        existingBooking.Topic = candidate.Topic;
        existingBooking.RoomId = candidate.RoomId;
        existingBooking.StartTime = candidate.StartTime;
        existingBooking.EndTime = candidate.EndTime;

        await _bookingRepository.SaveChangesAsync();
        _logger.LogInformation("Booking {BookingId} was updated.", existingBooking.BookingId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var booking = await GetBookingForCurrentUserAsync(id);
        if (booking is null)
        {
            _logger.LogWarning("Attempted to delete booking {BookingId}, but it was not found.", id);
            return NotFound();
        }

        await _bookingRepository.DeleteAsync(booking);
        _logger.LogInformation("Booking {BookingId} was deleted.", id);
        return RedirectToAction(nameof(Index));
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return string.IsNullOrEmpty(userId) ? null : await _userRepository.GetByUserIdAsync(userId);
    }

    private Task<Booking?> GetBookingForCurrentUserAsync(int id) =>
        _bookingRepository.GetByIdForUserAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier));

    private async Task<IActionResult> ReturnBookingFormAsync(BookingCreateViewModel viewModel)
    {
        viewModel.Rooms = await _roomRepository.GetAllAsync();
        return View(viewModel);
    }

    private void AddValidationErrors(BookingValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
        {
            ModelState.AddModelError(error.Key, error.Message);
        }
    }

    private static Booking CreateBooking(BookingCreateViewModel viewModel, User user) => new()
    {
        UserId = user.UserId,
        UserName = user.Name,
        Subject = viewModel.Subject,
        Topic = viewModel.Topic,
        RoomId = viewModel.RoomId,
        StartTime = viewModel.StartDate.ToDateTime(viewModel.StartClock),
        EndTime = viewModel.StartDate.ToDateTime(viewModel.EndClock)
    };
}
