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
        // Round up to the next 15-minute slot so the default matches a dropdown option
        var now = DateTime.Now;
        var startMinutes = (now.Hour * 60 + now.Minute + 14) / 15 * 15;
        var start = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(Math.Min(startMinutes, 23 * 60 + 45)));
        var end = TimeOnly.FromTimeSpan(
            TimeSpan.FromMinutes(Math.Min(start.Hour * 60 + start.Minute + 60, 23 * 60 + 45))
        );

        var viewModel = new BookingCreateViewModel
        {
            Rooms = _context.Rooms.ToList(),
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            StartClock = start,
            EndClock = end,
        };

        return View(viewModel);
    }

    // POST Create Booking
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BookingCreateViewModel viewModel)
    {
        var booking = viewModel.Booking;

        booking.StartTime = viewModel.StartDate.ToDateTime(viewModel.StartClock);
        booking.EndTime = viewModel.StartDate.ToDateTime(viewModel.EndClock);

        ValidateBooking(booking, viewModel, excludeId: null);

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
            StartDate = DateOnly.FromDateTime(booking.StartTime),
            StartClock = TimeOnly.FromDateTime(booking.StartTime),
            EndClock = TimeOnly.FromDateTime(booking.EndTime),
        };
        return View(viewModel);
    }

    // POST Edit Booking by Id
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, BookingCreateViewModel viewModel)
    {
        var booking = viewModel.Booking;

        if (id != booking.Id)
        {
            return NotFound();
        }

        booking.StartTime = viewModel.StartDate.ToDateTime(viewModel.StartClock);
        booking.EndTime = viewModel.StartDate.ToDateTime(viewModel.EndClock);

        ValidateBooking(booking, viewModel, excludeId: id);

        if (!ModelState.IsValid)
        {
            viewModel.Rooms = _context.Rooms.ToList();
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

    // Felles valideringslogikk for Create og Edit
    private void ValidateBooking(Booking booking, BookingCreateViewModel viewModel, int? excludeId)
    {
        if (booking.RoomId <= 0)
        {
            ModelState.AddModelError("Booking.RoomId", "Du må velge et rom.");
        }

        if (booking.EndTime <= booking.StartTime)
        {
            ModelState.AddModelError("EndClock", "Sluttid må være etter starttid.");
        }
        else
        {
            var duration = booking.EndTime - booking.StartTime;

            if (duration.TotalMinutes > 180)
            {
                ModelState.AddModelError(
                    "EndClock",
                    "En booking kan ikke vare lenger enn 3 timer."
                );
            }
        }

        if (booking.StartTime < DateTime.Now)
        {
            ModelState.AddModelError("StartClock", "Starttidspunkt kan ikke være i fortiden.");
        }

        bool hasConflict = _context.Bookings.Any(b =>
            b.RoomId == booking.RoomId
            && b.Id != excludeId
            && booking.StartTime < b.EndTime
            && booking.EndTime > b.StartTime
        );

        if (hasConflict)
        {
            ModelState.AddModelError("", "Rommet er allerede booket i dette tidsrommet.");
        }
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
