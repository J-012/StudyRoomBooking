
using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public class ParticipantRepository : IParticipantRepository
{
    private readonly StudyRoomDbContext _context;

    public ParticipantRepository(StudyRoomDbContext context)
    {
        _context = context;
    }

    // Find participant by the authenticated user's ID.
    public Task<Participant?> GetByUserIdAsync(string userId)
    {
        return _context.Participants
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    // Create a participant profile.
    public async Task AddAsync(Participant participant)
    {
        await _context.Participants.AddAsync(participant);
        await _context.SaveChangesAsync();
    }

    // Join a booking.
    public async Task AddToBookingAsync(
        BookingParticipant bookingParticipant)
    {
        await _context.BookingParticipants
            .AddAsync(bookingParticipant);

        await _context.SaveChangesAsync();
    }

    // Leave a booking.
    public async Task RemoveFromBookingAsync(
        BookingParticipant bookingParticipant)
    {
        _context.BookingParticipants.Remove(bookingParticipant);

        await _context.SaveChangesAsync();
    }
}
