using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Models;
using StudyRoomBooking.Services;
using StudyRoomBooking.ViewModels;
using StudyRoomBooking.ViewModels.Admin;

namespace StudyRoomBooking.Controllers;

public class RoomsController(
    IRoomService roomService,
    IBuildingService buildingService,
    IAmenityService amenityService,
    ILogger<RoomsController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] RoomsIndexViewModel filters)
    {
        filters.Buildings = (await buildingService.GetAllAsync()).Select(b => b.Name).ToList();

        if (filters.Date == default) filters.Date = DateTime.Today;

        if (filters.StartTime >= filters.EndTime)
            ModelState.AddModelError(nameof(filters.EndTime), "End time must be later than start time.");

        if (filters.Status is not ("all" or "available" or "booked"))
            ModelState.AddModelError(nameof(filters.Status), "Invalid room status.");

        if (!ModelState.IsValid)
        {
            filters.Rooms = new List<Room>();
            return View(filters);
        }

        var start = filters.Date.Date + filters.StartTime;
        var end = filters.Date.Date + filters.EndTime;
        filters.Rooms = await roomService.SearchAndFilterByAvailabilityAsync(
            filters.SearchTerm, filters.Building, filters.MinCapacity, start, end, filters.Status);

        return View(filters);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("/Admin/Rooms")]
    public async Task<IActionResult> AdminIndex([FromQuery] AdminRoomsViewModel filters)
    {
        filters.Buildings = await buildingService.GetAllAsync();
        filters.Amenities = await amenityService.GetAllAsync();

        if (ModelState.IsValid)
        {
            var matchingRooms = await roomService.SearchAndFilterAsync(
                filters.SearchTerm, filters.Building, filters.MinCapacity, filters.AmenityId);
            filters.TotalCount = matchingRooms.Count;
            // Keep the first results page at nine rooms, even if the query asks for fewer.
            filters.Show = Math.Max(9, filters.Show);
            filters.Rooms = matchingRooms.Take(filters.Show).ToList();
        }

        return View("~/Views/Admin/Rooms/Index.cshtml", filters);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("/Admin/Rooms/Create")]
    public async Task<IActionResult> AdminCreate() => RoomView("Create", await PopulateOptionsAsync(new RoomFormViewModel()));

    [Authorize(Roles = "Admin")]
    [HttpPost("/Admin/Rooms/Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminCreate([Bind("RoomNumber,BuildingId,Capacity,AmenityIds")] RoomFormViewModel input)
    {
        if (!ModelState.IsValid) return RoomView("Create", await PopulateOptionsAsync(input));

        var room = new Room
        {
            RoomNumber = input.RoomNumber.Trim(),
            BuildingId = input.BuildingId,
            Capacity = input.Capacity
        };
        var error = await roomService.SaveAsync(room, input.AmenityIds);
        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            return RoomView("Create", await PopulateOptionsAsync(input));
        }

        logger.LogInformation("Room {RoomId} created.", room.Id);
        TempData["Success"] = "Room added.";
        return RedirectToAction(nameof(AdminIndex));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("/Admin/Rooms/Edit/{id:int}")]
    public async Task<IActionResult> AdminEdit(int id)
    {
        var room = await roomService.GetByIdAsync(id);
        if (room is null) return NotFound();

        return RoomView("Edit", await PopulateOptionsAsync(new RoomFormViewModel
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            BuildingId = room.BuildingId,
            Capacity = room.Capacity,
            AmenityIds = room.RoomAmenities.Select(link => link.AmenityId).ToList()
        }));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("/Admin/Rooms/Edit/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminEdit(int id, [Bind("RoomNumber,BuildingId,Capacity,AmenityIds")] RoomFormViewModel input)
    {
        var room = await roomService.GetByIdAsync(id);
        if (room is null) return NotFound();
        input.Id = id;
        if (!ModelState.IsValid) return RoomView("Edit", await PopulateOptionsAsync(input));

        room.RoomNumber = input.RoomNumber.Trim();
        room.BuildingId = input.BuildingId;
        room.Capacity = input.Capacity;

        var error = await roomService.SaveAsync(room, input.AmenityIds);
        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            return RoomView("Edit", await PopulateOptionsAsync(input));
        }

        logger.LogInformation("Room {RoomId} updated.", id);
        TempData["Success"] = "Room updated.";
        return RedirectToAction(nameof(AdminIndex));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("/Admin/Rooms/Delete/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminDelete(int id)
    {
        var error = await roomService.DeleteForAdminAsync(id);
        TempData[error is null ? "Success" : "Error"] = error ?? "Room deleted.";
        if (error is null) logger.LogInformation("Room {RoomId} deleted.", id);
        else logger.LogWarning("Room {RoomId} deletion rejected: {Reason}", id, error);
        return RedirectToAction(nameof(AdminIndex));
    }

    private async Task<RoomFormViewModel> PopulateOptionsAsync(RoomFormViewModel input)
    {
        input.Buildings = await buildingService.GetAllAsync();
        input.Amenities = await amenityService.GetAllAsync();
        return input;
    }

    private ViewResult RoomView(string page, RoomFormViewModel model) =>
        View($"~/Views/Admin/Rooms/{page}.cshtml", model);
}
