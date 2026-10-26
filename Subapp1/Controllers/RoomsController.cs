using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Models;
using StudyRoomBooking.Services;

public class RoomsController : Controller
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    // GET all Rooms, with optional search and filters
    public async Task<IActionResult> Index(
        string? searchTerm,
        string? building,
        int? minCapacity)
    {
        var rooms = await _roomService.SearchAndFilterAsync(
            searchTerm,
            building,
            minCapacity);

        ViewBag.SearchTerm = searchTerm;
        ViewBag.Building = building;
        ViewBag.MinCapacity = minCapacity;

        return View(rooms);
    }

    // GET Room by Id
    public async Task<IActionResult> Details(int id)
    {
        var room = await _roomService.GetByIdAsync(id);

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
    // Bind limits the fields that can be posted, so users cannot set the room ID.
    public async Task<IActionResult> Create([Bind("RoomNumber,Building,Capacity")] Room room)
    {
        if (!ModelState.IsValid)
        {
            return View(room);
        }

        await _roomService.AddAsync(room);

        return RedirectToAction(nameof(Index));
    }

    // GET: /Rooms/Edit/5
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var room = await _roomService.GetByIdAsync(id);

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

        var room = await _roomService.GetByIdAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        room.RoomNumber = input.RoomNumber;
        room.Building = input.Building;
        room.Capacity = input.Capacity;

        await _roomService.UpdateAsync(room);

        return RedirectToAction(nameof(Index));
    }

    // POST: /Rooms/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var room = await _roomService.GetByIdAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        await _roomService.DeleteAsync(room);

        return RedirectToAction(nameof(Index));
    }
}
