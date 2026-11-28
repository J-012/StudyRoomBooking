
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

    public async Task AddUsersToBookingAsync(
        int bookingId,
        IReadOnlyCollection<User> users)
    {
        foreach (var user in users)
        {
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.UserId == user.UserId);

            if (participant is null)
            {
                participant = new Participant { UserId = user.UserId };
                _context.Participants.Add(participant);
            }

            _context.BookingParticipants.Add(new BookingParticipant
            {
                BookingId = bookingId,
                Participant = participant
            });
        }

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
