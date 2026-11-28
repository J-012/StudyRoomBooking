using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Models;
using StudyRoomBooking.Services;

namespace StudyRoomBooking.Controllers;

// Amenity forms live on the admin room list and edit pages.
[Authorize(Roles = "Admin")]
[Route("Admin/Amenities")]
public class AmenitiesController(IAmenityService service, ILogger<AmenitiesController> logger) : Controller
{
    [HttpPost("Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] Amenity input, [FromForm] int? returnRoomId)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = FirstValidationError();
            return BackToRooms(returnRoomId);
        }

        var error = await service.CreateAsync(input);
        TempData[error is null ? "Success" : "Error"] = error ??
            (returnRoomId is > 0 ? "Amenity added. Select it above and save the room to assign it." : "Amenity added.");
        if (error is null) logger.LogInformation("Amenity {AmenityId} created.", input.Id);
        return BackToRooms(returnRoomId);
    }

    [HttpPost("Edit/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Name,Description")] Amenity input, [FromForm] int? returnRoomId)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = FirstValidationError();
            return BackToRooms(returnRoomId);
        }

        var error = await service.UpdateAsync(id, input);
        TempData[error is null ? "Success" : "Error"] = error ?? "Amenity updated.";
        if (error is null) logger.LogInformation("Amenity {AmenityId} updated.", id);
        return BackToRooms(returnRoomId);
    }

    [HttpPost("Delete/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, [FromForm] int? returnRoomId)
    {
        var error = await service.DeleteAsync(id);
        TempData[error is null ? "Success" : "Error"] = error ?? "Amenity deleted.";
        if (error is null) logger.LogInformation("Amenity {AmenityId} deleted.", id);
        else logger.LogWarning("Amenity {AmenityId} deletion rejected: {Reason}", id, error);
        return BackToRooms(returnRoomId);
    }

    private string FirstValidationError() => ModelState.Values
        .SelectMany(value => value.Errors)
        .Select(error => error.ErrorMessage)
        .FirstOrDefault(message => !string.IsNullOrWhiteSpace(message)) ?? "Check the amenity details and try again.";

    private IActionResult BackToRooms(int? returnRoomId)
    {
        var url = returnRoomId is > 0
            ? Url.Action("AdminEdit", "Rooms", new { id = returnRoomId })
            : Url.Action("AdminIndex", "Rooms");

        return Redirect($"{url}#amenities");
    }
}
