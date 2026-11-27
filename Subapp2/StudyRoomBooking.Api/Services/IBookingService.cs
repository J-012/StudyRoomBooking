using StudyRoomBooking.Api.DTOs;

namespace StudyRoomBooking.Api.Services;

public interface IBookingService
{
    // The user's own sessions and the ones they have joined (admin: all).
    Task<List<BookingDto>> GetAllAsync(
        string currentUserId,
        bool isAdmin
    );

    // Every session that has not ended yet, so students can find one to join.
    Task<List<BookingDto>> GetOpenAsync(
        string currentUserId,
        bool isAdmin
    );

    Task<BookingDto?> GetByIdAsync(
        int id,
        string currentUserId,
        bool isAdmin
    );

    Task<BookingDto> CreateAsync(
        CreateBookingDto bookingDto,
        string currentUserId
    );

    Task<bool> UpdateAsync(
        int id,
        UpdateBookingDto bookingDto,
        string currentUserId,
        bool isAdmin
    );

    Task<bool> DeleteAsync(
        int id,
        string currentUserId,
        bool isAdmin
    );

    // Both return false if the session does not exist.
    Task<bool> JoinAsync(int id, string currentUserId);

    Task<bool> LeaveAsync(int id, string currentUserId);
}
