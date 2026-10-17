
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Models;
using System.Security.Claims;

namespace StudyRoomBooking.Controllers;

[Authorize]
public class BookingsController : Controller
{
    private readonly StudyRoomDbContext _context;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        StudyRoomDbContext context,
        ILogger<BookingsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Bookings
    public IActionResult Index()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var bookings = _context.Bookings
            .Where(b => b.StudentID == studentId)
            .ToList();

        return View(bookings);
    }

    // GET: Bookings/Details/{id}
    public IActionResult Details(int id)
    {
        // Finner studentens ID fra pålogget bruker
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Henter bookingen som tilhører den påloggede studenten
        var booking = _context.Bookings
            .FirstOrDefault(b => b.BookingId == id && b.StudentID == studentId);

        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }

    // GET: Bookings/Create/{id}
    public IActionResult Create()
    {
        // Round up to the next 15-minute slot so the default matches a dropdown option
        var now = DateTime.Now;
        var startMinutes = (now.Hour * 60 + now.Minute + 14) / 15 * 15;

        var start = TimeOnly.FromTimeSpan(
            TimeSpan.FromMinutes(Math.Min(startMinutes, 23 * 60 + 45)));

        var end = TimeOnly.FromTimeSpan(
            TimeSpan.FromMinutes(
                Math.Min(start.Hour * 60 + start.Minute + 60, 23 * 60 + 45)));

        var viewModel = new BookingCreateViewModel
        {
            Rooms = _context.Rooms.ToList(),
            StartDate = DateOnly.FromDateTime(now),
            StartClock = start,
            EndClock = end
        };

        return View(viewModel);
    }

    // POST: Bookings/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BookingCreateViewModel viewModel)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(studentId))
        {
            return Forbid();
        }

        // Ser om studenten eksisterer i databasen
        var student = _context.Students
            .FirstOrDefault(s => s.StudentId == studentId);

        if (student == null)
        {
            return Forbid();
        }
        //
        if (!ModelState.IsValid)
        {
            // Hent listen over rom på nytt
            viewModel.Rooms = _context.Rooms.ToList();
            return View(viewModel);
        }

        var booking = new Booking
        {
            // BookingId blir automatisk generert av databasen
            // Student info kommer fra databasen
            StudentID = student.StudentId,
            StudentName = student.Name,

            // Booking info kommer fra viewModel
            Subject = viewModel.Subject,
            Topic = viewModel.Topic,
            RoomId = viewModel.RoomId,
            StartTime = viewModel.StartDate.ToDateTime(viewModel.StartClock),
            EndTime = viewModel.StartDate.ToDateTime(viewModel.EndClock),
        };

        ValidateBooking(booking, excludeId: null);

        // Validerer igjen siden ValidateBooking() kan ha lagt til feil i ModelState
        if (!ModelState.IsValid)
        {
            viewModel.Rooms = _context.Rooms.ToList();
            return View(viewModel);
        }

        // Forteller at en ny booking skal legges til
        _context.Bookings.Add(booking);

        // Sender SQL-kommandoen til databasen
        _context.SaveChanges();

        _logger.LogInformation(
            "Booking {BookingId} created for room {RoomId}.",
            booking.BookingId,
            booking.RoomId);

        // Redirect til oversikten over bookinger etter at lagringen er fullført
        return RedirectToAction(nameof(Index));
    }

    // GET: Bookings/Edit/{id}
    public IActionResult Edit(int id)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Hent bookingen som skal redigeres fra databasen basert på ID og studentID
        var booking = _context.Bookings
            .FirstOrDefault(b => b.BookingId == id && b.StudentID == studentId);

        if (booking == null)
        {
            return NotFound();
        }

        var viewModel = new BookingCreateViewModel
        {
            BookingId = booking.BookingId,
            Subject = booking.Subject,
            Topic = booking.Topic,
            RoomId = booking.RoomId,
            Rooms = _context.Rooms.ToList(),

            // Hent eksisterende dato og klokkeslett fra databasen
            StartDate = DateOnly.FromDateTime(booking.StartTime),
            StartClock = TimeOnly.FromDateTime(booking.StartTime),
            EndClock = TimeOnly.FromDateTime(booking.EndTime)
        };

        return View(viewModel);
    }

    // POST: Bookings/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, BookingCreateViewModel viewModel)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var existingBooking = _context.Bookings
            .FirstOrDefault(b => b.BookingId == id && b.StudentID == studentId);

        if (existingBooking == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            viewModel.Rooms = _context.Rooms.ToList();
            return View(viewModel);
        }

        // Lag en ny booking basert på de feltene studenten har lov til å endre
        var booking = new Booking
        {
            BookingId = existingBooking.BookingId,
            StudentID = existingBooking.StudentID,
            StudentName = existingBooking.StudentName,
            Subject = viewModel.Subject,
            Topic = viewModel.Topic,
            RoomId = viewModel.RoomId,
            StartTime = viewModel.StartDate.ToDateTime(viewModel.StartClock),
            EndTime = viewModel.StartDate.ToDateTime(viewModel.EndClock),
        };

        // Validerer den nye bookingen før lagring, ekskluderer den eksisterende bookingen fra konfliktkontrollen
        ValidateBooking(booking, excludeId: id);

        if (!ModelState.IsValid)
        {
            viewModel.Rooms = _context.Rooms.ToList();
            return View(viewModel);
        }

        // Oppdaterer kun feltene studenten har lov til å endre
        existingBooking.Subject = booking.Subject;
        existingBooking.Topic = booking.Topic;
        existingBooking.RoomId = booking.RoomId;
        existingBooking.StartTime = booking.StartTime;
        existingBooking.EndTime = booking.EndTime;

        _context.SaveChanges();

        _logger.LogInformation(
            "Booking {BookingId} was updated.",
            existingBooking.BookingId);

        return RedirectToAction(nameof(Index));
    }

    // Felles valideringslogikk for Create og Edit
    private void ValidateBooking(Booking booking, int? excludeId)
    {
        // Validerer at rommet eksisterer og er gyldig
        if (booking.RoomId <= 0 ||
            !_context.Rooms.Any(r => r.Id == booking.RoomId))
        {
            ModelState.AddModelError(
                nameof(booking.RoomId),
                "Du må velge et gyldig rom.");
        }

        // Validerer at sluttid er etter starttid og at varigheten ikke overstiger 3 timer
        if (booking.EndTime <= booking.StartTime)
        {
            ModelState.AddModelError(
                "EndClock",
                "Sluttid må være etter starttid.");
        }
        else if ((booking.EndTime - booking.StartTime).TotalMinutes > 180)
        {
            ModelState.AddModelError(
                "EndClock",
                "En booking kan ikke vare lenger enn 3 timer.");
        }

        // Validerer at starttidspunktet ikke er i fortiden
        if (booking.StartTime < DateTime.Now)
        {
            ModelState.AddModelError(
                "StartClock",
                "Starttidspunkt kan ikke være i fortiden.");
        }

        var hasConflict = _context.Bookings.Any(b =>
            b.RoomId == booking.RoomId &&
            b.BookingId != excludeId &&
            booking.StartTime < b.EndTime &&
            booking.EndTime > b.StartTime);

        if (hasConflict)
        {
            ModelState.AddModelError(
                string.Empty,
                "Rommet er allerede booket i dette tidsrommet.");
        }
    }

    // POST: Bookings/Delete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        // Henter ID-en til den innloggede studenten
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Sjekker at bookingen tilhører studenten
        var booking = _context.Bookings
            .FirstOrDefault(b => b.BookingId == id && b.StudentID == studentId);

        if (booking == null)
        {
            _logger.LogWarning(
                "Attempted to delete booking {BookingId}, but it was not found.",
                id);

            return NotFound();
        }

        _context.Bookings.Remove(booking);
        _context.SaveChanges();

        _logger.LogInformation(
            "Booking {BookingId} was deleted.",
            id);

        return RedirectToAction(nameof(Index));
    }
}
