using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Api.DAL;
using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly StudyRoomBookingDbContext _context;

    public BookingRepository(
        StudyRoomBookingDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudySession>> GetAllAsync()
    {
        return await _context.StudySessions
            .Include(session => session.Room)
            .Include(session => session.Organizer)
            .AsNoTracking()
            .OrderBy(session => session.StartTime)
            .ToListAsync();
    }

    public async Task<StudySession?> GetByIdAsync(int id)
    {
        return await _context.StudySessions
            .Include(session => session.Room)
            .Include(session => session.Organizer)
            .FirstOrDefaultAsync(
                session => session.Id == id
            );
    }

    public async Task<StudySession> AddAsync(
        StudySession session)
    {
        await _context.StudySessions.AddAsync(session);

        await _context.SaveChangesAsync();

        StudySession? createdSession =
            await GetByIdAsync(session.Id);

        return createdSession ?? session;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        StudySession session)
    {
        _context.StudySessions.Remove(session);

        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasConflictAsync(
        int roomId,
        DateTime startTime,
        DateTime endTime,
        int? excludedSessionId)
    {
        return await _context.StudySessions
            .AnyAsync(session =>
                session.RoomId == roomId
                &&
                (!excludedSessionId.HasValue
                    || session.Id != excludedSessionId.Value)
                &&
                startTime < session.EndTime
                &&
                endTime > session.StartTime
            );
    }
}