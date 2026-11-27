using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Repositories;

namespace StudyRoomBooking.Api.Services;

public class BookingService : IBookingService
{
    private const int MaxActiveBookingsPerUser = 3;

    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
    }

    public async Task<List<BookingDto>> GetAllAsync(
        string currentUserId,
        bool isAdmin)
    {
        List<StudySession> sessions =
            await _bookingRepository.GetAllAsync();

        List<BookingDto> bookingDtos = new List<BookingDto>();

        foreach (StudySession session in sessions)
        {
            // Studenter ser egne bookinger og økter de har blitt med i,
            // admin ser alle.
            if (!CanSee(session, currentUserId, isAdmin))
            {
                continue;
            }

            bookingDtos.Add(ToDto(session, currentUserId, isAdmin));
        }

        return bookingDtos;
    }

    public async Task<List<BookingDto>> GetOpenAsync(
        string currentUserId,
        bool isAdmin)
    {
        List<StudySession> sessions =
            await _bookingRepository.GetUpcomingAsync();

        List<BookingDto> bookingDtos = new List<BookingDto>();

        foreach (StudySession session in sessions)
        {
            bookingDtos.Add(ToDto(session, currentUserId, isAdmin));
        }

        return bookingDtos;
    }

    public async Task<BookingDto?> GetByIdAsync(
        int id,
        string currentUserId,
        bool isAdmin)
    {
        StudySession? session =
            await _bookingRepository.GetByIdAsync(id);

        // Andres bookinger behandles som "finnes ikke" (404),
        // så man ikke kan hente dem ved å gjette en id.
        if (session == null
            || !CanSee(session, currentUserId, isAdmin))
        {
            return null;
        }

        return ToDto(session, currentUserId, isAdmin);
    }

    public async Task<BookingDto> CreateAsync(
        CreateBookingDto bookingDto,
        string currentUserId)
    {
        int activeBookingCount =
            await _bookingRepository
                .CountActiveBookingsForUserAsync(currentUserId);

        if (activeBookingCount >= MaxActiveBookingsPerUser)
        {
            throw new ArgumentException(
                $"You can have a maximum of {MaxActiveBookingsPerUser} active bookings at a time."
            );
        }

        await ValidateAsync(
            bookingDto.RoomId,
            bookingDto.StartTime,
            bookingDto.EndTime,
            null
        );

        StudySession session = new StudySession
        {
            Subject = bookingDto.Subject,
            Topic = bookingDto.Topic,
            RoomId = bookingDto.RoomId,
            StartTime = bookingDto.StartTime,
            EndTime = bookingDto.EndTime,
            OrganizerUserId = currentUserId
        };

        StudySession createdSession =
            await _bookingRepository.AddAsync(session);

        return ToDto(createdSession, currentUserId, false);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateBookingDto bookingDto,
        string currentUserId,
        bool isAdmin)
    {
        StudySession? session =
            await _bookingRepository.GetByIdAsync(id);

        if (session == null)
        {
            return false;
        }

        if (!CanManage(session, currentUserId, isAdmin))
        {
            throw new UnauthorizedAccessException(
                "You can only edit your own bookings."
            );
        }

        Room room = await ValidateAsync(
            bookingDto.RoomId,
            bookingDto.StartTime,
            bookingDto.EndTime,
            id
        );

        // The session may be moved to another room, but not to one that is
        // too small for the people who have already joined.
        if (CountAttendees(session) > room.Capacity)
        {
            throw new ArgumentException(
                "The selected room is too small for the people who have joined this session."
            );
        }

        session.Subject = bookingDto.Subject;
        session.Topic = bookingDto.Topic;
        session.RoomId = bookingDto.RoomId;
        session.StartTime = bookingDto.StartTime;
        session.EndTime = bookingDto.EndTime;

        await _bookingRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        string currentUserId,
        bool isAdmin)
    {
        StudySession? session =
            await _bookingRepository.GetByIdAsync(id);

        if (session == null)
        {
            return false;
        }

        if (!CanManage(session, currentUserId, isAdmin))
        {
            throw new UnauthorizedAccessException(
                "You can only delete your own bookings."
            );
        }

        await _bookingRepository.DeleteAsync(session);

        return true;
    }

    public async Task<bool> JoinAsync(int id, string currentUserId)
    {
        StudySession? session =
            await _bookingRepository.GetByIdAsync(id);

        if (session == null)
        {
            return false;
        }

        if (session.EndTime < DateTime.Now)
        {
            throw new ArgumentException(
                "This session has already ended."
            );
        }

        if (session.OrganizerUserId == currentUserId)
        {
            throw new ArgumentException(
                "You are the organizer of this session."
            );
        }

        if (IsParticipant(session, currentUserId))
        {
            throw new ArgumentException(
                "You have already joined this session."
            );
        }

        // The room's capacity limits how many can attend. The organizer
        // takes one of the seats.
        int capacity = session.Room?.Capacity ?? 0;

        if (CountAttendees(session) >= capacity)
        {
            throw new ArgumentException(
                "This session is full."
            );
        }

        await _bookingRepository.AddParticipantAsync(
            new SessionParticipant
            {
                StudySessionId = session.Id,
                UserId = currentUserId
            }
        );

        return true;
    }

    public async Task<bool> LeaveAsync(int id, string currentUserId)
    {
        StudySession? session =
            await _bookingRepository.GetByIdAsync(id);

        if (session == null)
        {
            return false;
        }

        SessionParticipant? participant =
            session.Participants.FirstOrDefault(
                participant => participant.UserId == currentUserId
            );

        if (participant == null)
        {
            throw new ArgumentException(
                "You have not joined this session."
            );
        }

        await _bookingRepository.RemoveParticipantAsync(participant);

        return true;
    }

    // Throws ArgumentException if a rule is broken, otherwise returns the room.
    private async Task<Room> ValidateAsync(
        int roomId,
        DateTime startTime,
        DateTime endTime,
        int? excludedSessionId)
    {
        Room? room =
            await _roomRepository.GetByIdAsync(roomId);

        if (room == null)
        {
            throw new ArgumentException(
                "You must select a valid room."
            );
        }

        if (endTime <= startTime)
        {
            throw new ArgumentException(
                "The end time must be after the start time."
            );
        }

        double duration =
            (endTime - startTime).TotalMinutes;

        if (duration < 15)
        {
            throw new ArgumentException(
                "A booking must last at least 15 minutes."
            );
        }

        if (duration > 180)
        {
            throw new ArgumentException(
                "A booking cannot last longer than three hours."
            );
        }

        if (startTime < DateTime.Now)
        {
            throw new ArgumentException(
                "The start time cannot be in the past."
            );
        }

        bool hasConflict =
            await _bookingRepository.HasConflictAsync(
                roomId,
                startTime,
                endTime,
                excludedSessionId
            );

        if (hasConflict)
        {
            throw new ArgumentException(
                "The room is already booked during this time."
            );
        }

        return room;
    }

    private bool CanManage(
        StudySession session,
        string currentUserId,
        bool isAdmin)
    {
        return isAdmin
            || session.OrganizerUserId == currentUserId;
    }

    private bool IsParticipant(
        StudySession session,
        string currentUserId)
    {
        return session.Participants.Any(
            participant => participant.UserId == currentUserId
        );
    }

    private bool CanSee(
        StudySession session,
        string currentUserId,
        bool isAdmin)
    {
        return CanManage(session, currentUserId, isAdmin)
            || IsParticipant(session, currentUserId);
    }

    // The organizer plus everyone who has joined.
    private int CountAttendees(StudySession session)
    {
        int organizer = session.OrganizerUserId == null ? 0 : 1;

        return session.Participants.Count + organizer;
    }

    private BookingDto ToDto(
        StudySession session,
        string currentUserId,
        bool isAdmin)
    {
        return new BookingDto
        {
            Id = session.Id,
            Subject = session.Subject,
            Topic = session.Topic,
            RoomId = session.RoomId,
            RoomNumber =
                session.Room?.RoomNumber ?? string.Empty,
            Building =
                session.Room?.Building?.Name ?? string.Empty,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            OrganizerName =
                session.Organizer?.FullName ?? "Unknown",
            CanManage =
                CanManage(session, currentUserId, isAdmin),
            Capacity = session.Room?.Capacity ?? 0,
            AttendeeCount = CountAttendees(session),
            IsOrganizer =
                session.OrganizerUserId == currentUserId,
            IsParticipant =
                IsParticipant(session, currentUserId)
        };
    }
}
