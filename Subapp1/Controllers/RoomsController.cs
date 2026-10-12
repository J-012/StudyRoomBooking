using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Models;

public class RoomsController : Controller
{

    private readonly StudyRoomDbContext _context;
    public RoomsController(StudyRoomDbContext context)
    {
        _context = context;
    }

    // GET all Rooms
    public IActionResult Index()
    {
        var rooms = _context.Rooms.ToList();
        return View(rooms);
    }

    // GET Room by Id
    public IActionResult Details(int id)
    {
        var room = _context.Rooms.FirstOrDefault(r => r.Id == id);

        if (room == null)
        {
            return NotFound();
        }

        return View(room);
    }
}
    