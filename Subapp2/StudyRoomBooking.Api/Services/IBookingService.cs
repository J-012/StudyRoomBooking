using StudyRoomBooking.Api.DTOs;

namespace StudyRoomBooking.Api.Services;

public interface IBookingService
{
    Task<List<BookingDto>> GetAllAsync(
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
}