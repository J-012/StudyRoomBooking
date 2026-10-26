using StudyRoomBooking.Models;
using StudyRoomBooking.Repositories;

namespace StudyRoomBooking.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public Task<List<Course>> GetAllAsync()
    {
        return _courseRepository.GetAllAsync();
    }

    public Task<Course?> GetByCourseCodeAsync(string courseCode)
    {
        return _courseRepository.GetByCourseCodeAsync(NormalizeCourseCode(courseCode));
    }

    // CREATE
    public async Task<string?> CreateAsync(Course course)
    {
        var error = ValidateCourse(course);

        if (error != null)
            return error;

        course.CourseCode = course.CourseCode.Trim().ToUpperInvariant();
        course.Name = course.Name.Trim();
        course.Description = course.Description?.Trim() ?? string.Empty;

        bool exists = await _courseRepository.CourseCodeExistsAsync(
            course.CourseCode);

        if (exists)
            return "A course with this code already exists.";

        await _courseRepository.AddAsync(course);

        return null;
    }

    // UPDATE
    public async Task<string?> UpdateAsync(string courseCode, Course input)
    {
        var normalizedCourseCode = NormalizeCourseCode(courseCode);
        var course = await _courseRepository.GetByCourseCodeAsync(normalizedCourseCode);

        if (course == null)
            return "Course not found.";

        var error = ValidateCourse(input);

        if (error != null)
            return error;

        if (!string.Equals(input.CourseCode.Trim(), normalizedCourseCode, StringComparison.OrdinalIgnoreCase))
            return "Course code cannot be changed because it is the course identifier.";

        course.Name = input.Name.Trim();
        course.Description = input.Description?.Trim() ?? string.Empty;

        await _courseRepository.UpdateAsync(course);

        return null;
    }

    // DELETE
    public async Task<string?> DeleteAsync(string courseCode)
    {
        var normalizedCourseCode = NormalizeCourseCode(courseCode);
        var course = await _courseRepository.GetByCourseCodeAsync(normalizedCourseCode);

        if (course == null)
            return "Course not found.";

        bool hasBookings = await _courseRepository.HasBookingsAsync(normalizedCourseCode);

        if (hasBookings)
            return "Cannot delete a course with existing bookings.";

        await _courseRepository.DeleteAsync(course);

        return null;
    }

    // Felles validering for Create og Update
    private static string? ValidateCourse(Course course)
    {
        if (string.IsNullOrWhiteSpace(course.CourseCode))
            return "Course code is required.";

        if (course.CourseCode.Trim().Length > 20)
            return "Course code cannot exceed 20 characters.";

        if (string.IsNullOrWhiteSpace(course.Name))
            return "Course name is required.";

        if (course.Name.Trim().Length > 100)
            return "Course name cannot exceed 100 characters.";

        if ((course.Description?.Trim().Length ?? 0) > 500)
            return "Description cannot exceed 500 characters.";

        return null;
    }

    private static string NormalizeCourseCode(string courseCode) =>
        courseCode.Trim().ToUpperInvariant();
}
