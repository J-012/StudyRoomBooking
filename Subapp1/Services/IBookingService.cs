using StudyRoomBooking.Models;

namespace StudyRoomBooking.Services;

public interface IBookingService
{
    Task<BookingValidationResult> ValidateAsync(Booking booking, int? excludedBookingId);
}
