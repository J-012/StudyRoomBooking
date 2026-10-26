using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly StudyRoomDbContext _context;

    public BookingRepository(StudyRoomDbContext context)
    {
        _context = context;
    }

    // Get all bookings for a specific user by their user ID
    public Task<List<Booking>> GetByUserIdAsync(string userId)
    {
        return _context.Bookings
            .AsNoTracking()
            .Include(booking => booking.Course)
            .Where(booking => booking.UserId == userId)
            .ToListAsync();
    }

    // Get a specific booking for a user by booking ID and user ID.
    public Task<Booking?> GetByIdForUserAsync(int bookingId, string? userId)
    {
        return _context.Bookings.FirstOrDefaultAsync(booking =>
            booking.BookingId == bookingId && booking.UserId == userId);
    }

    // Check whether a booking conflicts with existing bookings.
    // excludedBookingId excludes a specific booking from the conflict check, for example during an update.
    public Task<bool> HasConflictAsync(Booking booking, int? excludedBookingId)
    {
        return _context.Bookings.AnyAsync(existingBooking =>
            existingBooking.RoomId == booking.RoomId &&
            existingBooking.BookingId != excludedBookingId &&
            booking.StartTime < existingBooking.EndTime &&
            booking.EndTime > existingBooking.StartTime);
    }

    // Add a new booking.
    public async Task<Booking> AddAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return booking;
    }

    // Save changes to the database.
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    // Delete an existing booking.
    public async Task DeleteAsync(Booking booking)
    {
        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
    }
}
