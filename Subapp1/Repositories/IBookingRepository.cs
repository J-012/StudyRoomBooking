using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public interface IBookingRepository
{
    // Get all bookings for a specific user by their user ID
    Task<List<Booking>> GetByUserIdAsync(string userId);

    // Få et spesifikt booking for en bruker basert på booking ID og bruker ID
    Task<Booking?> GetByIdForUserAsync(int bookingId, string? userId);

    // Sjekk om en booking har konflikt med eksisterende bookinger
    // excludedBookingId brukes for å ekskludere en spesifikk booking fra konflikten (f.eks. når man oppdaterer en booking)
    Task<bool> HasConflictAsync(Booking booking, int? excludedBookingId);

    // Legg til en ny booking
    Task<Booking> AddAsync(Booking booking);

    // Lagre endringer i databasen
    Task SaveChangesAsync();

    // Slett en eksisterende booking
    Task DeleteAsync(Booking booking);
}
