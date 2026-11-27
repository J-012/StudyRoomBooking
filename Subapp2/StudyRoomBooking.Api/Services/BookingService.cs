using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Repositories;

namespace StudyRoomBooking.Api.Services;

public class BookingService : IBookingService
{
    // Booking rules.
    private const int MaxActiveBookingsPerUser = 3;
    private const int MinDurationMinutes = 15;
    private const int MaxDurationMinutes = 180;

    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
    }

    public async Task<List<BookingDto>> GetAllAsync(string currentUserId, bool isAdmin)
    {
        // Admins see every booking (null = no filter).
        List<StudySession> sessions =
            await _bookingRepository.GetAllAsync(isAdmin ? null : currentUserId);

        return sessions.Select(session => ToDto(session, currentUserId, isAdmin)).ToList();
    }

    public async Task<List<BookingDto>> GetOpenAsync(string currentUserId, bool isAdmin)
    {
        List<StudySession> sessions = await _bookingRepository.GetUpcomingAsync();

        return sessions.Select(session => ToDto(session, currentUserId, isAdmin)).ToList();
    }

    public async Task<BookingDto?> GetByIdAsync(int id, string currentUserId, bool isAdmin)
    {
        StudySession? session = await _bookingRepository.GetByIdAsync(id);

        // Other people's bookings are treated as "not found" (404),
        // so nobody can fetch them by guessing an id.
        if (session == null || !CanSee(session, currentUserId, isAdmin))
        {
            return null;
        }

        return ToDto(session, currentUserId, isAdmin);
    }

    public async Task<BookingDto> CreateAsync(BookingRequestDto bookingDto, string currentUserId)
    {
        int activeBookings = await _bookingRepository.CountActiveBookingsForUserAsync(currentUserId);

        if (activeBookings >= MaxActiveBookingsPerUser)
        {
            throw new ArgumentException(
                $"You can have a maximum of {MaxActiveBookingsPerUser} active bookings at a time.");
        }

        await ValidateAsync(bookingDto, null);

        StudySession session = new StudySession
        {
            Subject = bookingDto.Subject,
            Topic = bookingDto.Topic,
            RoomId = bookingDto.RoomId,
            StartTime = bookingDto.StartTime,
            EndTime = bookingDto.EndTime,
            OrganizerUserId = currentUserId
        };

        StudySession createdSession = await _bookingRepository.AddAsync(session);

        return ToDto(createdSession, currentUserId, false);
    }

    public async Task<bool> UpdateAsync(
        int id,
        BookingRequestDto bookingDto,
        string currentUserId,
        bool isAdmin)
    {
        StudySession? session = await GetForManageAsync(id, currentUserId, isAdmin, "edit");

        if (session == null)
        {
            return false;
        }

        Room room = await ValidateAsync(bookingDto, id);

        // The session may be moved to another room, but not to one that is
        // too small for the people who have already joined.
        if (CountAttendees(session) > room.Capacity)
        {
            throw new ArgumentException(
                "The selected room is too small for the people who have joined this session.");
        }

        session.Subject = bookingDto.Subject;
        session.Topic = bookingDto.Topic;
        session.RoomId = bookingDto.RoomId;
        session.StartTime = bookingDto.StartTime;
        session.EndTime = bookingDto.EndTime;

        await _bookingRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id, string currentUserId, bool isAdmin)
    {
        StudySession? session = await GetForManageAsync(id, currentUserId, isAdmin, "delete");

        if (session == null)
        {
            return false;
        }

        await _bookingRepository.DeleteAsync(session);

        return true;
    }

    public async Task<bool> JoinAsync(int id, string currentUserId)
    {
        StudySession? session = await _bookingRepository.GetByIdAsync(id);

        if (session == null)
        {
            return false;
        }

        if (session.EndTime < DateTime.Now)
        {
            throw new ArgumentException("This session has already ended.");
        }

        if (session.OrganizerUserId == currentUserId)
        {
            throw new ArgumentException("You are the organizer of this session.");
        }

        if (IsParticipant(session, currentUserId))
        {
            throw new ArgumentException("You have already joined this session.");
        }

        // The room's capacity limits how many can attend. The organizer takes one seat.
        if (CountAttendees(session) >= (session.Room?.Capacity ?? 0))
        {
            throw new ArgumentException("This session is full.");
        }

        await _bookingRepository.AddParticipantAsync(new SessionParticipant
        {
            StudySessionId = session.Id,
            UserId = currentUserId
        });

        return true;
    }

    public async Task<bool> LeaveAsync(int id, string currentUserId)
    {
        StudySession? session = await _bookingRepository.GetByIdAsync(id);

        if (session == null)
        {
            return false;
        }

        SessionParticipant? participant =
            session.Participants.FirstOrDefault(p => p.UserId == currentUserId);

        if (participant == null)
        {
            throw new ArgumentException("You have not joined this session.");
        }

        await _bookingRepository.RemoveParticipantAsync(participant);

        return true;
    }

    // Shared by update and delete. Returns null if the session does not exist,
    // and throws if it belongs to someone else.
    private async Task<StudySession?> GetForManageAsync(
        int id,
        string currentUserId,
        bool isAdmin,
        string action)
    {
        StudySession? session = await _bookingRepository.GetByIdAsync(id);

        if (session != null && !CanManage(session, currentUserId, isAdmin))
        {
            throw new UnauthorizedAccessException($"You can only {action} your own bookings.");
        }

        return session;
    }

    // Shared by create and update. Throws ArgumentException if a rule is broken,
    // otherwise returns the room.
    private async Task<Room> ValidateAsync(BookingRequestDto bookingDto, int? excludedSessionId)
    {
        Room? room = await _roomRepository.GetByIdAsync(bookingDto.RoomId);

        if (room == null)
        {
            throw new ArgumentException("You must select a valid room.");
        }

        if (bookingDto.EndTime <= bookingDto.StartTime)
        {
            throw new ArgumentException("The end time must be after the start time.");
        }

        double duration = (bookingDto.EndTime - bookingDto.StartTime).TotalMinutes;

        if (duration < MinDurationMinutes)
        {
            throw new ArgumentException($"A booking must last at least {MinDurationMinutes} minutes.");
        }

        if (duration > MaxDurationMinutes)
        {
            throw new ArgumentException("A booking cannot last longer than three hours.");
        }

        if (bookingDto.StartTime < DateTime.Now)
        {
            throw new ArgumentException("The start time cannot be in the past.");
        }

        bool hasConflict = await _bookingRepository.HasConflictAsync(
            bookingDto.RoomId,
            bookingDto.StartTime,
            bookingDto.EndTime,
            excludedSessionId);

        if (hasConflict)
        {
            throw new ArgumentException("The room is already booked during this time.");
        }

        return room;
    }

    // Organizer or admin may edit and delete.
    private static bool CanManage(StudySession session, string currentUserId, bool isAdmin) =>
        isAdmin || session.OrganizerUserId == currentUserId;

    private static bool IsParticipant(StudySession session, string currentUserId) =>
        session.Participants.Any(participant => participant.UserId == currentUserId);

    private static bool CanSee(StudySession session, string currentUserId, bool isAdmin) =>
        CanManage(session, currentUserId, isAdmin) || IsParticipant(session, currentUserId);

    // The organizer plus everyone who has joined.
    private static int CountAttendees(StudySession session) =>
        session.Participants.Count + (session.OrganizerUserId == null ? 0 : 1);

    private static BookingDto ToDto(StudySession session, string currentUserId, bool isAdmin) => new()
    {
        Id = session.Id,
        Subject = session.Subject,
        Topic = session.Topic,
        RoomId = session.RoomId,
        RoomNumber = session.Room?.RoomNumber ?? string.Empty,
        Building = session.Room?.Building?.Name ?? string.Empty,
        StartTime = session.StartTime,
        EndTime = session.EndTime,
        OrganizerName = session.Organizer?.FullName ?? "Unknown",
        Capacity = session.Room?.Capacity ?? 0,
        AttendeeCount = CountAttendees(session),
        CanManage = CanManage(session, currentUserId, isAdmin),
        IsOrganizer = session.OrganizerUserId == currentUserId,
        IsParticipant = IsParticipant(session, currentUserId)
    };
}
