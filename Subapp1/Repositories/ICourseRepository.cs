using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public interface ICourseRepository
{
    Task<List<Course>> GetAllAsync();
    Task<Course?> GetByCourseCodeAsync(string courseCode);

    Task<bool> CourseCodeExistsAsync(string courseCode);

    Task<bool> HasBookingsAsync(string courseCode);

    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(Course course);
}
