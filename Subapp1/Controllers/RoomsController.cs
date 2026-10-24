using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Repositories;

public class RoomsController : Controller
{

    private readonly IRoomRepository _roomRepository;
    public RoomsController(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    // GET all Rooms
    public async Task<IActionResult> Index()
    {
        var rooms = await _roomRepository.GetAllAsync();
        return View(rooms);
    }

    // GET Room by Id
    public async Task<IActionResult> Details(int id)
    {
        var room = await _roomRepository.GetByIdAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        return View(room);
    }
}
