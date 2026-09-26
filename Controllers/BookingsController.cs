using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Controllers;

public class BookingsController : Controller
{
    private readonly StudyRoomDbContext _context;

    public BookingsController(StudyRoomDbContext context)
    {
        _context = context;
    }

    // Next available booking ID

    // GET all Bookings
    public IActionResult Index()
    {
        return View(_context.Bookings.ToList());
    }

    // GET Booking by Id
    public IActionResult Details(int id)
    {
        var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);

        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
    }

    // GET Create Booking
    public IActionResult Create()
    {
        var viewModel = new BookingCreateViewModel
        {
            // Default booking info, brukt for å fylle ut skjemaet raskere
            Booking = new Booking()
            {
                Subject = "Algorithms and Data Structures",
                Topic = "Binary Trees",
                StudentName = "Eyad Laza",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
            },

            Rooms = _context.Rooms.ToList(),
        };

        return View(viewModel);
    }

    // POST Create Booking
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BookingCreateViewModel viewModel)
    {
        var booking = viewModel.Booking;

        if (booking.EndTime <= booking.StartTime)
        {
            ModelState.AddModelError("Booking.EndTime", "End time must be after start time.");
        }

        if (!ModelState.IsValid)
        {
            viewModel.Rooms = _context.Rooms.ToList();
            return View(viewModel);
        }

        _context.Bookings.Add(booking);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    // GET Edit Booking by Id
    public IActionResult Edit(int id)
    {
        var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);

        if (booking == null)
        {
            return NotFound();
        }

        var viewModel = new BookingCreateViewModel
        {
            Booking = booking,
            Rooms = _context.Rooms.ToList(),
        };
        return View(viewModel);
    }

    // POST Edit Booking by Id
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Booking booking)
    {
        if (id != booking.Id)
        {
            return NotFound();
        }

        if (booking.EndTime <= booking.StartTime)
        {
            ModelState.AddModelError("EndTime", "End time must be after start time.");
        }

        if (!ModelState.IsValid)
        {
            var viewModel = new BookingCreateViewModel
            {
                Booking = booking,
                Rooms = _context.Rooms.ToList(),
            };
            // Returnerer det brukeren har fylt ut i skjemaet slik at de kan rette feilene enklere
            return View(viewModel);
        }

        var existingBooking = _context.Bookings.FirstOrDefault(b => b.Id == id);

        if (existingBooking == null)
        {
            return NotFound();
        }

        existingBooking.Subject = booking.Subject;
        existingBooking.Topic = booking.Topic;
        existingBooking.StudentName = booking.StudentName;
        existingBooking.RoomId = booking.RoomId;
        existingBooking.StartTime = booking.StartTime;
        existingBooking.EndTime = booking.EndTime;

        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    // POST Delete Booking by Id
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);

        if (booking == null)
        {
            return NotFound();
        }

        _context.Bookings.Remove(booking);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}
