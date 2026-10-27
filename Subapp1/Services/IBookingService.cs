using StudyRoomBooking.Models;

namespace StudyRoomBooking.Services;

public interface IBookingService
{
    Task<BookingValidationResult> ValidateAsync(Booking booking, int? excludedBookingId);

    Task<JoinBookingResult> JoinAsync(
        int bookingId,
        string userId);

    Task<JoinBookingResult> LeaveAsync(
        int bookingId,
        string userId);
}
