using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Repositories;

public interface IBookingRepository
{
    Task<List<StudySession>> GetAllAsync();

    Task<StudySession?> GetByIdAsync(int id);

    Task<StudySession> AddAsync(StudySession session);

    Task SaveChangesAsync();

    Task DeleteAsync(StudySession session);

    Task<bool> HasConflictAsync(
        int roomId,
        DateTime startTime,
        DateTime endTime,
        int? excludedSessionId
    );
}