using StudyRoomBooking.Api.DTOs;

namespace StudyRoomBooking.Api.Services;

// Methods return null/false when the booking does not exist

public interface IBookingService
{
    // The user's own sessions and the ones they have joine
    Task<List<BookingDto>> GetAllAsync(string currentUserId, bool isAdmin);

    // Every session that has not ended yet, so students can find one to join.
    Task<List<BookingDto>> GetOpenAsync(string currentUserId, bool isAdmin);

    Task<BookingDto?> GetByIdAsync(int id, string currentUserId, bool isAdmin);

    Task<BookingDto> CreateAsync(BookingRequestDto bookingDto, string currentUserId);

    Task<bool> UpdateAsync(int id, BookingRequestDto bookingDto, string currentUserId, bool isAdmin);

    Task<bool> DeleteAsync(int id, string currentUserId, bool isAdmin);

    // The current user joins / leaves someone else's session.
    Task<bool> JoinAsync(int id, string currentUserId);

    Task<bool> LeaveAsync(int id, string currentUserId);
}
