using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Repositories;

namespace StudyRoomBooking.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin")]
public class AdminController(IAdminOverviewRepository repository) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index() => View("~/Views/Admin/Overview/Index.cshtml", await repository.GetOverviewAsync());
}
