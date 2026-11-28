using StudyRoomBooking.Models;
using StudyRoomBooking.Repositories;

namespace StudyRoomBooking.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IParticipantRepository _participantRepository;

    // Constructor for BookingService.
    public BookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IParticipantRepository participantRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _participantRepository = participantRepository;
    }

    // Validate a booking.
    public async Task<BookingValidationResult> ValidateAsync(
        Booking booking,
        int? excludedBookingId)
    {
        var result = new BookingValidationResult();

        if (booking.RoomId <= 0 || !await _roomRepository.ExistsAsync(booking.RoomId))
        {
            result.AddError(nameof(booking.RoomId), "You must select a valid room.");
        }

        if (booking.EndTime <= booking.StartTime)
        {
            result.AddError("EndClock", "The end time must be after the start time.");
        }
        else if ((booking.EndTime - booking.StartTime).TotalMinutes < 15)
        {
            result.AddError("EndClock", "A booking must last at least 15 minutes.");
        }
        else if ((booking.EndTime - booking.StartTime).TotalMinutes > 180)
        {
            result.AddError("EndClock", "A booking cannot last longer than three hours.");
        }

        if (booking.StartTime < DateTime.Now)
        {
            result.AddError("StartClock", "The start time cannot be in the past.");
        }

        if (await _bookingRepository.HasConflictAsync(booking, excludedBookingId))
        {
            result.AddError(string.Empty, "The room is already booked during this time.");
        }

        return result;
    }

    // Join a booking.
    public async Task<JoinBookingResult> JoinAsync(
    int bookingId,
    string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return JoinBookingResult.Failure(
                "You must be logged in to join a session.");
        }

        var booking = await _bookingRepository
            .GetWithParticipantsAsync(bookingId);

        if (booking == null)
        {
            return JoinBookingResult.Failure(
                "Booking not found.");
        }

        if (booking.StartTime <= DateTime.Now)
        {
            return JoinBookingResult.Failure(
                "You cannot join a session that has already started.");
        }

        if (booking.UserId == userId)
        {
            return JoinBookingResult.Failure(
                "You are already the owner of this booking.");
        }

        if (booking.Participants.Any(
            bp => bp.Participant.UserId == userId))
        {
            return JoinBookingResult.Failure(
                "You have already joined this session.");
        }

        if (!booking.IsPublic)
        {
            return JoinBookingResult.Failure(
                "This session is not available to join.");
        }

        // Booking owner counts as one participant.
        int occupiedSeats = booking.Participants.Count + 1;

        if (booking.Room == null)
        {
            return JoinBookingResult.Failure(
                "Room not found.");
        }

        if (occupiedSeats >= booking.Room.Capacity)
        {
            return JoinBookingResult.Failure(
                "This study session is full.");
        }

        // Find or create the participant profile.
        var participant = await _participantRepository
            .GetByUserIdAsync(userId);

        if (participant == null)
        {
            participant = new Participant
            {
                UserId = userId
            };

            await _participantRepository.AddAsync(participant);
        }

        var bookingParticipant = new BookingParticipant
        {
            BookingId = bookingId,
            ParticipantId = participant.ParticipantId,
            JoinedAt = DateTime.UtcNow
        };

        await _participantRepository
            .AddToBookingAsync(bookingParticipant);

        return JoinBookingResult.Success();
    }

    public async Task<JoinBookingResult> AddParticipantsAsync(
        int bookingId,
        string ownerUserId,
        IReadOnlyCollection<User> users)
    {
        if (string.IsNullOrWhiteSpace(ownerUserId))
        {
            return JoinBookingResult.Failure(
                "You must be logged in to add session participants.");
        }

        var booking = await _bookingRepository.GetWithParticipantsAsync(bookingId);
        if (booking is null || booking.UserId != ownerUserId)
        {
            return JoinBookingResult.Failure("Session not found.");
        }

        if (booking.IsPublic)
        {
            return JoinBookingResult.Failure(
                "Make the session private before adding participants.");
        }

        if (booking.EndTime <= DateTime.Now)
        {
            return JoinBookingResult.Failure(
                "You cannot add participants after the session has ended.");
        }

        if (booking.Room is null)
        {
            return JoinBookingResult.Failure("Room not found.");
        }

        var requestedUsers = users
            .Where(user => !string.IsNullOrWhiteSpace(user.UserId))
            .GroupBy(user => user.UserId.Trim(), StringComparer.Ordinal)
            .Select(group => group.First())
            .ToList();

        if (requestedUsers.Count == 0)
        {
            return JoinBookingResult.Failure(
                "Enter at least one User ID.");
        }

        if (requestedUsers.Any(user => user.UserId == booking.UserId))
        {
            return JoinBookingResult.Failure(
                "The session owner is already part of the session.");
        }

        var existingUserIds = booking.Participants
            .Select(participant => participant.Participant.UserId)
            .ToHashSet(StringComparer.Ordinal);
        var newUsers = requestedUsers
            .Where(user => !existingUserIds.Contains(user.UserId))
            .ToList();

        if (newUsers.Count == 0)
        {
            return JoinBookingResult.Failure(
                "Those users are already in the session.");
        }

        int occupiedSeats = booking.Participants.Count + 1;
        if (occupiedSeats + newUsers.Count > booking.Room.Capacity)
        {
            return JoinBookingResult.Failure(
                "There are not enough seats for all selected users.");
        }

        await _participantRepository.AddUsersToBookingAsync(
            bookingId,
            newUsers);

        return JoinBookingResult.Success();
    }

    // Leave a booking.
    public async Task<JoinBookingResult> LeaveAsync(
    int bookingId,
    string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return JoinBookingResult.Failure(
                "You must be logged in.");
        }

        var booking = await _bookingRepository
            .GetWithParticipantsAsync(bookingId);

        if (booking == null)
        {
            return JoinBookingResult.Failure(
                "Booking not found.");
        }

        if (booking.UserId == userId)
        {
            return JoinBookingResult.Failure(
                "The booking owner cannot leave their own session.");
        }

        var bookingParticipant = booking.Participants
            .FirstOrDefault(bp =>
                bp.Participant.UserId == userId);

        if (bookingParticipant == null)
        {
            return JoinBookingResult.Failure(
                "You are not a participant in this session.");
        }

        await _participantRepository
            .RemoveFromBookingAsync(bookingParticipant);

        return JoinBookingResult.Success();
    }
}
