using StudyRoomBooking.Models;

namespace StudyRoomBooking.Services;

public interface ICourseService
{
    Task<List<Course>> GetAllAsync();
    Task<Course?> GetByCourseCodeAsync(string courseCode);

    Task<string?> CreateAsync(Course course);
    Task<string?> UpdateAsync(string courseCode, Course input);
    Task<string?> DeleteAsync(string courseCode);
}
