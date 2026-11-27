using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Api.DAL;
using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly StudyRoomBookingDbContext _context;

    public BookingRepository(StudyRoomBookingDbContext context)
    {
        _context = context;
    }

    // Sessions with everything BookingDto needs. Shared by all read methods.
    private IQueryable<StudySession> SessionsWithDetails =>
        _context.StudySessions
            .Include(session => session.Room)
                .ThenInclude(room => room!.Building)
            .Include(session => session.Organizer)
            .Include(session => session.Participants);

    // Sessions that overlap the time range [startTime, endTime).
    // Two ranges overlap when each one starts before the other ends.
    private IQueryable<StudySession> Overlapping(
        DateTime startTime,
        DateTime endTime,
        int? excludedSessionId) =>
        _context.StudySessions.Where(session =>
            startTime < session.EndTime
            && endTime > session.StartTime
            && (excludedSessionId == null || session.Id != excludedSessionId));

    public Task<List<StudySession>> GetAllAsync(string? userId) =>
        SessionsWithDetails
            .AsNoTracking()
            .Where(session => userId == null
                || session.OrganizerUserId == userId
                || session.Participants.Any(participant => participant.UserId == userId))
            .OrderBy(session => session.StartTime)
            .ToListAsync();

    public Task<List<StudySession>> GetUpcomingAsync()
    {
        DateTime now = DateTime.Now;

        return SessionsWithDetails
            .AsNoTracking()
            .Where(session => session.EndTime >= now)
            .OrderBy(session => session.StartTime)
            .ToListAsync();
    }

    public Task<StudySession?> GetByIdAsync(int id) =>
        SessionsWithDetails.FirstOrDefaultAsync(session => session.Id == id);

    public async Task<StudySession> AddAsync(StudySession session)
    {
        _context.StudySessions.Add(session);
        await _context.SaveChangesAsync();

        // Reload so Room, Building and Organizer are filled in for the response.
        return (await GetByIdAsync(session.Id))!;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public async Task DeleteAsync(StudySession session)
    {
        _context.StudySessions.Remove(session);
        await _context.SaveChangesAsync();
    }

    public async Task AddParticipantAsync(SessionParticipant participant)
    {
        _context.SessionParticipants.Add(participant);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveParticipantAsync(SessionParticipant participant)
    {
        _context.SessionParticipants.Remove(participant);
        await _context.SaveChangesAsync();
    }

    public Task<int> CountActiveBookingsForUserAsync(string organizerUserId)
    {
        DateTime now = DateTime.Now;

        return _context.StudySessions.CountAsync(session =>
            session.OrganizerUserId == organizerUserId
            && session.EndTime >= now);
    }

    public Task<bool> HasUpcomingBookingsForRoomAsync(int roomId)
    {
        DateTime now = DateTime.Now;

        return _context.StudySessions.AnyAsync(session =>
            session.RoomId == roomId
            && session.EndTime >= now);
    }

    public Task<bool> HasConflictAsync(
        int roomId,
        DateTime startTime,
        DateTime endTime,
        int? excludedSessionId) =>
        Overlapping(startTime, endTime, excludedSessionId)
            .AnyAsync(session => session.RoomId == roomId);

    public Task<List<int>> GetBookedRoomIdsAsync(
        DateTime startTime,
        DateTime endTime,
        int? excludedSessionId) =>
        Overlapping(startTime, endTime, excludedSessionId)
            .Select(session => session.RoomId)
            .Distinct()
            .ToListAsync();
}
