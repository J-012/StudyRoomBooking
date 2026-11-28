using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Models;
using StudyRoomBooking.Services;

namespace StudyRoomBooking.Controllers;

public class CoursesController(ICourseService courseService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index() => View(await courseService.GetAllAsync());

    [Authorize(Roles = "Admin")]
    [HttpGet("/Admin/Courses")]
    public async Task<IActionResult> AdminIndex() => CourseView("Index", await courseService.GetAllAsync());

    [Authorize(Roles = "Admin")]
    [HttpGet("/Admin/Courses/Create")]
    public IActionResult AdminCreate() => CourseView("Create");

    [Authorize(Roles = "Admin")]
    [HttpPost("/Admin/Courses/Create"), ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminCreate([Bind("CourseCode,Name,Description")] Course course)
    {
        if (!ModelState.IsValid) return CourseView("Create", course);

        var error = await courseService.CreateAsync(course);
        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            return CourseView("Create", course);
        }

        TempData["Success"] = "Course created successfully!";
        return RedirectToAction(nameof(AdminIndex));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("/Admin/Courses/Edit")]
    public async Task<IActionResult> AdminEdit(string courseCode)
    {
        var course = await courseService.GetByCourseCodeAsync(courseCode);
        return course is null ? NotFound() : CourseView("Edit", course);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("/Admin/Courses/Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminEdit(string courseCode, [Bind("CourseCode,Name,Description")] Course input)
    {
        var existingCourse = await courseService.GetByCourseCodeAsync(courseCode);
        if (existingCourse is null) return NotFound();
        if (!ModelState.IsValid) return CourseView("Edit", input);

        var error = await courseService.UpdateAsync(courseCode, input);
        if (error is not null)
        {
            // Keep the route key unchanged when redisplaying a failed edit.
            input.CourseCode = existingCourse.CourseCode;
            ModelState.Remove(nameof(input.CourseCode));
            ModelState.AddModelError(string.Empty, error);
            return CourseView("Edit", input);
        }

        TempData["Success"] = "Course updated successfully!";
        return RedirectToAction(nameof(AdminIndex));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("/Admin/Courses/Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminDelete(string courseCode)
    {
        var error = await courseService.DeleteAsync(courseCode);
        TempData[error is null ? "Success" : "Error"] = error ?? "Course deleted successfully!";
        return RedirectToAction(nameof(AdminIndex));
    }

    private ViewResult CourseView(string page, object? model = null) =>
        View($"~/Views/Admin/Courses/{page}.cshtml", model);
}
