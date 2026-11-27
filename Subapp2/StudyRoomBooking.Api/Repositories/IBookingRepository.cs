using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Repositories;

public interface IBookingRepository
{
    Task<List<StudySession>> GetAllAsync();

    // Sessions that have not ended yet, earliest first.
    Task<List<StudySession>> GetUpcomingAsync();

    Task<StudySession?> GetByIdAsync(int id);

    Task<StudySession> AddAsync(StudySession session);

    Task SaveChangesAsync();

    Task DeleteAsync(StudySession session);

    Task AddParticipantAsync(SessionParticipant participant);

    Task RemoveParticipantAsync(SessionParticipant participant);

    Task<int> CountActiveBookingsForUserAsync(string organizerUserId);

    // True if the room has at least one booking that has not ended yet.
    Task<bool> HasUpcomingBookingsForRoomAsync(int roomId);

    Task<bool> HasConflictAsync(
        int roomId,
        DateTime startTime,
        DateTime endTime,
        int? excludedSessionId
    );
}
