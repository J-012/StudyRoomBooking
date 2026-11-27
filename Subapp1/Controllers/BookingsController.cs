using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Models;
using StudyRoomBooking.Repositories;
using StudyRoomBooking.Services;
using StudyRoomBooking.ViewModels;

namespace StudyRoomBooking.Controllers;

[Authorize]
public class BookingsController : Controller
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBookingService _bookingService;
    private readonly ICourseService _courseService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IUserRepository userRepository,
        IBookingService bookingService,
        ILogger<BookingsController> logger,
        ICourseService courseService
    )
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _bookingService = bookingService;
        _logger = logger;
        _courseService = courseService;
    }

    // GET: /Bookings
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // Get the current user's ID from the claims.
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // If the user ID is not found in the claims, return Unauthorized.
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var viewModel = new BookingsIndexViewModel
        {
            MyBookings = await _bookingRepository.GetByUserIdAsync(userId),
            JoinedBookings = await _bookingRepository.GetJoinedByUserIdAsync(userId),
            JoinableBookings = await _bookingRepository.GetJoinableBookingsAsync(userId)
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create(
        int? roomId,
        DateOnly? date,
        TimeOnly? startTime,
        TimeOnly? endTime)
    {
        if (!roomId.HasValue || !date.HasValue || !startTime.HasValue || !endTime.HasValue)
        {
            TempData["Error"] = "Choose an available room, date, and time before creating a study session.";
            return RedirectToAction("Index", "Rooms");
        }

        var selectedRoom = await _roomRepository.GetByIdAsync(roomId.Value);
        var candidate = new Booking
        {
            RoomId = roomId.Value,
            StartTime = date.Value.ToDateTime(startTime.Value),
            EndTime = date.Value.ToDateTime(endTime.Value)
        };

        var validation = await _bookingService.ValidateAsync(candidate, null);
        if (selectedRoom is null || !validation.IsValid)
        {
            TempData["Error"] = validation.Errors.FirstOrDefault()?.Message ??
                "That room is no longer available for the selected time.";
            return RedirectToRooms(date, startTime, endTime);
        }

        var viewModel = new BookingFormViewModel
        {
            Courses = await _courseService.GetAllAsync(),
            SelectedRoom = selectedRoom,
            RoomId = selectedRoom.Id,
            StartDate = date.Value,
            StartClock = startTime.Value,
            EndClock = endTime.Value
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("CourseCode,Topic,RoomId,StartDate,StartClock,EndClock")] BookingFormViewModel viewModel)
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

        var course = await _courseService.GetByCourseCodeAsync(viewModel.CourseCode);

        if (course is null)
        {
            ModelState.AddModelError(nameof(viewModel.CourseCode), "Please select a valid course.");

            return await ReturnBookingFormAsync(viewModel);
        }

        var booking = CreateBooking(viewModel, user);

        AddValidationErrors(await _bookingService.ValidateAsync(booking, null));

        if (!ModelState.IsValid)
        {
            return await ReturnBookingFormAsync(viewModel);
        }

        await _bookingRepository.AddAsync(booking);

        _logger.LogInformation(
            "Booking {BookingId} created for room {RoomId} and course {CourseCode}.",
            booking.BookingId,
            booking.RoomId,
            booking.CourseCode
        );

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var booking = await GetBookingForCurrentUserAsync(id);

        if (booking is null)
        {
            return NotFound();
        }

        var viewModel = new BookingFormViewModel
        {
            BookingId = booking.BookingId,

            CourseCode = booking.CourseCode ?? string.Empty,
            Topic = booking.Topic,
            RoomId = booking.RoomId,

            Rooms = await _roomRepository.GetAllAsync(),
            Courses = await _courseService.GetAllAsync(),

            StartDate = DateOnly.FromDateTime(booking.StartTime),

            StartClock = TimeOnly.FromDateTime(booking.StartTime),

            EndClock = TimeOnly.FromDateTime(booking.EndTime),
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookingFormViewModel viewModel)
    {
        var existingBooking = await GetBookingForCurrentUserAsync(id);

        if (existingBooking is null)
        {
            return NotFound();
        }

        viewModel.BookingId = id;

        if (!ModelState.IsValid)
        {
            return await ReturnBookingFormAsync(viewModel);
        }

        var course = await _courseService.GetByCourseCodeAsync(viewModel.CourseCode);

        if (course is null)
        {
            ModelState.AddModelError(nameof(viewModel.CourseCode), "Please select a valid course.");

            return await ReturnBookingFormAsync(viewModel);
        }

        var candidate = new Booking
        {
            BookingId = existingBooking.BookingId,
            UserId = existingBooking.UserId,
            UserName = existingBooking.UserName,

            CourseCode = course.CourseCode,
            Topic = viewModel.Topic,
            RoomId = viewModel.RoomId,

            StartTime = viewModel.StartDate.ToDateTime(viewModel.StartClock),

            EndTime = viewModel.StartDate.ToDateTime(viewModel.EndClock),
        };

        AddValidationErrors(await _bookingService.ValidateAsync(candidate, id));

        if (!ModelState.IsValid)
        {
            return await ReturnBookingFormAsync(viewModel);
        }

        existingBooking.CourseCode = candidate.CourseCode;
        existingBooking.Topic = candidate.Topic;
        existingBooking.RoomId = candidate.RoomId;
        existingBooking.StartTime = candidate.StartTime;
        existingBooking.EndTime = candidate.EndTime;

        await _bookingRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Booking {BookingId} was updated. Course: {CourseCode}.",
            existingBooking.BookingId,
            existingBooking.CourseCode
        );

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var booking = await GetBookingForCurrentUserAsync(id);

        if (booking is null)
        {
            _logger.LogWarning(
                "Attempted to delete booking {BookingId}, but it was not found.",
                id
            );

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

    private Task<Booking?> GetBookingForCurrentUserAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return _bookingRepository.GetByIdForUserAsync(id, userId);
    }

    private async Task<IActionResult> ReturnBookingFormAsync(BookingFormViewModel viewModel)
    {
        viewModel.SelectedRoom = await _roomRepository.GetByIdAsync(viewModel.RoomId);
        viewModel.Courses = await _courseService.GetAllAsync();

        return View(viewModel);
    }

    private void AddValidationErrors(BookingValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
        {
            ModelState.AddModelError(error.Key, error.Message);
        }
    }

    private static Booking CreateBooking(BookingFormViewModel viewModel, User user)
    {
        return new Booking
        {
            UserId = user.UserId,
            UserName = user.Name,

            CourseCode = viewModel.CourseCode.Trim().ToUpperInvariant(),
            Topic = viewModel.Topic,
            RoomId = viewModel.RoomId,

            StartTime = viewModel.StartDate.ToDateTime(viewModel.StartClock),

            EndTime = viewModel.StartDate.ToDateTime(viewModel.EndClock),
        };
    }

    private IActionResult RedirectToRooms(DateOnly? date, TimeOnly? startTime, TimeOnly? endTime)
    {
        return RedirectToAction("Index", "Rooms", new
        {
            date,
            startTime,
            endTime
        });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Join(int id, bool returnToRooms = false)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var result = await _bookingService.JoinAsync(id, userId);

        if (result.IsSuccess)
        {
            TempData["Success"] = "You successfully joined the study session.";

            _logger.LogInformation(
                "User {UserId} joined booking {BookingId}.",
                userId,
                id
            );
        }
        else
        {
            TempData["Error"] = result.Error ?? "Unable to join the session.";

            _logger.LogWarning(
                "User {UserId} could not join booking {BookingId}: {Reason}",
                userId,
                id,
                result.Error
            );
        }

        return returnToRooms
            ? RedirectToAction("Index", "Rooms")
            : RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Leave(int id, bool returnToRooms = false)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Challenge();
        }

        var result = await _bookingService.LeaveAsync(id, userId);

        if (result.IsSuccess)
        {
            TempData["Success"] = "You successfully left the study session.";

            _logger.LogInformation(
                "User {UserId} left booking {BookingId}.",
                userId,
                id
            );
        }
        else
        {
            TempData["Error"] = result.Error ?? "Unable to leave the session.";

            _logger.LogWarning(
                "User {UserId} could not leave booking {BookingId}: {Reason}",
                userId,
                id,
                result.Error
            );
        }

        return returnToRooms
            ? RedirectToAction("Index", "Rooms")
            : RedirectToAction(nameof(Index));
    }

}
