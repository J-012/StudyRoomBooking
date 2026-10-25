using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Models;
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

    // GET: /Rooms/Create
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Rooms/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([Bind("RoomNumber,Building,Capacity")] Room room) // Bruker Bind for å begrense hvilke felter som kan settes via POST, f.eks brukeren bør ikke kunne fylle id
    {
        if (!ModelState.IsValid)
        {
            return View(room);
        }

        await _roomRepository.AddAsync(room);

        return RedirectToAction(nameof(Index));
    }

    // GET: /Rooms/Edit/5
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var room = await _roomRepository.GetByIdAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        return View(room);
    }

    // POST: /Rooms/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("RoomNumber,Building,Capacity")] Room input)
    {
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        var room = await _roomRepository.GetByIdAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        room.RoomNumber = input.RoomNumber;
        room.Building = input.Building;
        room.Capacity = input.Capacity;

        await _roomRepository.UpdateAsync(room);

        return RedirectToAction(nameof(Index));
    }

    // POST: /Rooms/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var room = await _roomRepository.GetByIdAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        await _roomRepository.DeleteAsync(room);

        return RedirectToAction(nameof(Index));
    }
}
