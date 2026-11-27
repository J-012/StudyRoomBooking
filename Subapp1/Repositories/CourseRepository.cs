using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly StudyRoomDbContext _context;

    public CourseRepository(StudyRoomDbContext context)
    {
        _context = context;
    }

    // Hent alle emner
    public Task<List<Course>> GetAllAsync()
    {
        return _context.Courses
            .AsNoTracking()
            .OrderBy(c => c.CourseCode)
            .ToListAsync();
    }

    // Hent ett emne
    public Task<Course?> GetByCourseCodeAsync(string courseCode)
    {
        return _context.Courses
            .FirstOrDefaultAsync(c => c.CourseCode == courseCode);
    }

    // Sjekk om emnekoden finnes fra før
    public Task<bool> CourseCodeExistsAsync(
        string courseCode)
    {
        return _context.Courses.AnyAsync(c => c.CourseCode == courseCode);
    }

    // Sjekk om emnet er knyttet til bookinger
    public Task<bool> HasBookingsAsync(string courseCode)
    {
        return _context.Bookings.AnyAsync(b =>
            b.CourseCode == courseCode);
    }

    // Opprett emne
    public async Task AddAsync(Course course)
    {
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
    }

    // Oppdater emne
    public async Task UpdateAsync(Course course)
    {
        await _context.SaveChangesAsync();
    }

    // Slett emne
    public async Task DeleteAsync(Course course)
    {
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
    }
}
