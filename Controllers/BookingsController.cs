using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Controllers;

public class BookingsController : Controller
{
    private static List<Booking> bookings = new();

    // Next available booking ID
    private static int nextId = 1;

    // GET all Bookings
    public IActionResult Index()
    {
        return View(bookings);
    }

    // GET Booking by Id
    public IActionResult Details(int id)
    {
        var booking = bookings.FirstOrDefault(b => b.Id == id);

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
                Id = nextId++,
                Subject = "Algorithms and Data Structures",
                Topic = "Binary Trees",
                StudentName = "Eyad Laza",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
            },
            // Må byttes med rom som kommer fra databasen
            Rooms = RoomsController.Rooms,
        };

        return View(viewModel);
    }

    // POST Create Booking
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Booking booking)
    {
        if (booking.EndTime <= booking.StartTime)
        {
            ModelState.AddModelError("EndTime", "End time must be after start time.");
        }

        if (!ModelState.IsValid)
        {
            var viewModel = new BookingCreateViewModel
            {
                Booking = booking,
                Rooms = RoomsController.Rooms,
            };
            return View(viewModel);
        }

        booking.Id = nextId++;
        bookings.Add(booking);

        return RedirectToAction(nameof(Index));
    }

    // GET Edit Booking by Id
    public IActionResult Edit(int id)
    {
        var booking = bookings.FirstOrDefault(b => b.Id == id);

        if (booking == null)
        {
            return NotFound();
        }

        return View(booking);
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
                Rooms = RoomsController.Rooms,
            };
            return View(viewModel);
        }

        var existingBooking = bookings.FirstOrDefault(b => b.Id == id);

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

        return RedirectToAction(nameof(Index));
    }

    // POST Delete Booking by Id
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var booking = bookings.FirstOrDefault(b => b.Id == id);

        if (booking == null)
        {
            return NotFound();
        }

        bookings.Remove(booking);

        return RedirectToAction(nameof(Index));
    }
}
