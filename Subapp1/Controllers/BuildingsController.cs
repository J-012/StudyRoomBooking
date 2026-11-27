using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Models;
using StudyRoomBooking.Services;

namespace StudyRoomBooking.Controllers;

[Route("Admin/Buildings")]
[Authorize(Roles = "Admin")]
public class BuildingsController(IBuildingService service, ILogger<BuildingsController> logger) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index() => BuildingView("Index", await service.GetAllAsync());

    [HttpGet("Create")]
    public IActionResult Create() => BuildingView("Create", new Building());

    [HttpPost("Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Address")] Building input)
    {
        if (!ModelState.IsValid) return BuildingView("Create", input);
        var error = await service.CreateAsync(input);
        if (error is not null)
        {
            ModelState.AddModelError(nameof(input.Name), error);
            return BuildingView("Create", input);
        }
        logger.LogInformation("Building {BuildingId} created.", input.Id);
        TempData["Success"] = "Building added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var building = await service.GetByIdAsync(id);
        return building is null ? NotFound() : BuildingView("Edit", building);
    }

    [HttpPost("Edit/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Name,Address")] Building input)
    {
        if (await service.GetByIdAsync(id) is null) return NotFound();
        input.Id = id;
        if (!ModelState.IsValid) return BuildingView("Edit", input);
        var error = await service.UpdateAsync(id, input);
        if (error is not null)
        {
            ModelState.AddModelError(nameof(input.Name), error);
            return BuildingView("Edit", input);
        }
        logger.LogInformation("Building {BuildingId} updated.", id);
        TempData["Success"] = "Building updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var error = await service.DeleteAsync(id);
        if (error is null) logger.LogInformation("Building {BuildingId} deleted.", id);
        else logger.LogWarning("Building {BuildingId} deletion rejected: {Reason}", id, error);
        TempData[error is null ? "Success" : "Error"] = error ?? "Building deleted.";
        return RedirectToAction(nameof(Index));
    }

    private ViewResult BuildingView(string page, object model) =>
        View($"~/Views/Admin/Buildings/{page}.cshtml", model);
}
