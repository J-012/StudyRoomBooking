using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Repositories;

// Sessions are always returned with Room, Building, Organizer and Participants loaded.
public interface IBookingRepository
{
    Task<List<StudySession>> GetAllAsync(string? userId);

    // Sessions that have not ended yet, earliest first.
    Task<List<StudySession>> GetUpcomingAsync();

    Task<StudySession?> GetByIdAsync(int id);

    Task<StudySession> AddAsync(StudySession session);

    Task SaveChangesAsync();

    Task DeleteAsync(StudySession session);

    Task AddParticipantAsync(SessionParticipant participant);

    Task RemoveParticipantAsync(SessionParticipant participant);

    // Bookings the user organizes that have not ended yet.
    Task<int> CountActiveBookingsForUserAsync(string organizerUserId);

    // True if the room has at least one booking that has not ended yet.
    Task<bool> HasUpcomingBookingsForRoomAsync(int roomId);

    // True if the room is booked at any point between startTime and endTime.
    Task<bool> HasConflictAsync(int roomId, DateTime startTime, DateTime endTime, int? excludedSessionId);

    // Ids of all rooms that are booked at any point between startTime and endTime.
    Task<List<int>> GetBookedRoomIdsAsync(DateTime startTime, DateTime endTime, int? excludedSessionId);
}
