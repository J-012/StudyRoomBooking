using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public interface IParticipantRepository
{
    Task<Participant?> GetByUserIdAsync(string userId);

    Task AddAsync(Participant participant);

    Task AddToBookingAsync(
        BookingParticipant bookingParticipant);

    Task AddUsersToBookingAsync(
        int bookingId,
        IReadOnlyCollection<User> users);

    Task RemoveFromBookingAsync(
        BookingParticipant bookingParticipant);
}