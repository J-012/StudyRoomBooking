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
        return _context.Bookings.Where(booking => booking.UserId == userId).ToListAsync();
    }

    // Få et spesifikt booking for en bruker basert på booking ID og bruker ID
    public Task<Booking?> GetByIdForUserAsync(int bookingId, string? userId)
    {
        return _context.Bookings.FirstOrDefaultAsync(booking =>
            booking.BookingId == bookingId && booking.UserId == userId);
    }

    // Sjekk om en booking har konflikt med eksisterende bookinger
    // excludedBookingId brukes for å ekskludere en spesifikk booking fra konflikten (f.eks. når man oppdaterer en booking)
    public Task<bool> HasConflictAsync(Booking booking, int? excludedBookingId)
    {
        return _context.Bookings.AnyAsync(existingBooking =>
            existingBooking.RoomId == booking.RoomId &&
            existingBooking.BookingId != excludedBookingId &&
            booking.StartTime < existingBooking.EndTime &&
            booking.EndTime > existingBooking.StartTime);
    }

    // Legg til en ny booking
    public async Task<Booking> AddAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return booking;
    }

    // Lagre endringer i databasen
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    // Slett en eksisterende booking
    public async Task DeleteAsync(Booking booking)
    {
        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
    }
}
