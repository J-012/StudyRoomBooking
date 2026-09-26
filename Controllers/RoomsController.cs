using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Models;

public class RoomsController : Controller
{
    // Test data for roommene
    public static List<Room> Rooms = new()
    {
        new Room
        {
            Id = 1,
            RoomNumber = "101",
            Building = "Pilestredet 35",
            Capacity = 6,
        },
        new Room
        {
            Id = 2,
            RoomNumber = "102",
            Building = "Pilestredet 35",
            Capacity = 4,
        },
        new Room
        {
            Id = 3,
            RoomNumber = "203",
            Building = "Pilestredet 46",
            Capacity = 8,
        },
    };

    // GET all Rooms
    public IActionResult Index()
    {
        return View(Rooms);
    }

    // GET Room by Id
    public IActionResult Details(int id)
    {
        var room = Rooms.FirstOrDefault(r => r.Id == id);

        if (room == null)
        {
            return NotFound();
        }

        return View(room);
    }
}
