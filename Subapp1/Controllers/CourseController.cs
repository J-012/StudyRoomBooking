using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Models;
using StudyRoomBooking.Services;

namespace StudyRoomBooking.Controllers;

public class CoursesController : Controller
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    // GET: /Courses
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var courses = await _courseService.GetAllAsync();
        return View(courses);
    }

    // GET: /Courses/Details?courseCode=CS101
    [HttpGet]
    public async Task<IActionResult> Details(string courseCode)
    {
        var course = await _courseService.GetByCourseCodeAsync(courseCode);

        if (course == null)
            return NotFound();

        return View(course);
    }

    // GET: /Courses/Create
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Courses/Create
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("CourseCode,Name,Description")] Course course)
    {
        if (!ModelState.IsValid)
            return View(course);

        var error = await _courseService.CreateAsync(course);

        if (error != null)
        {
            ModelState.AddModelError("", error);
            return View(course);
        }

        TempData["Success"] = "Course created successfully!";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Courses/Edit?courseCode=CS101
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(string courseCode)
    {
        var course = await _courseService.GetByCourseCodeAsync(courseCode);

        if (course == null)
            return NotFound();

        return View(course);
    }

    // POST: /Courses/Edit?courseCode=CS101
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        string courseCode,
        [Bind("CourseCode,Name,Description")] Course input)
    {
        var existingCourse = await _courseService.GetByCourseCodeAsync(courseCode);

        if (existingCourse == null)
            return NotFound();

        if (!ModelState.IsValid)
            return View(input);

        var error = await _courseService.UpdateAsync(courseCode, input);

        if (error != null)
        {
            input.CourseCode = existingCourse.CourseCode;
            ModelState.Remove(nameof(input.CourseCode));
            ModelState.AddModelError("", error);
            return View(input);
        }

        TempData["Success"] = "Course updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Courses/Delete?courseCode=CS101
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string courseCode)
    {
        var error = await _courseService.DeleteAsync(courseCode);

        if (error != null)
            TempData["Error"] = error;
        else
            TempData["Success"] = "Course deleted successfully!";

        return RedirectToAction(nameof(Index));
    }
}
