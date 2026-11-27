using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public interface IBookingRepository
{
    // Get all bookings for a specific user by their user ID
    Task<List<Booking>> GetByUserIdAsync(string userId);

    Task<List<Booking>> GetJoinedByUserIdAsync(string userId);

    Task<List<Booking>> GetJoinableBookingsAsync(string userId);

    // Get a specific booking for a user by booking ID and user ID.
    Task<Booking?> GetByIdForUserAsync(int bookingId, string? userId);

    // Check whether a booking conflicts with existing bookings.
    // excludedBookingId excludes a specific booking from the conflict check, for example during an update.
    Task<bool> HasConflictAsync(Booking booking, int? excludedBookingId);

    // Add a new booking.
    Task<Booking> AddAsync(Booking booking);

    // Save changes to the database.
    Task SaveChangesAsync();

    // Delete an existing booking.
    Task DeleteAsync(Booking booking);

        Task<Booking?> GetWithParticipantsAsync(int bookingId);

}
