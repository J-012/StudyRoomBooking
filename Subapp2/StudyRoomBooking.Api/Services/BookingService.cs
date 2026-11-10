using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Repositories;

namespace StudyRoomBooking.Api.Services;

public class BookingService : IBookingService
{
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

        List<BookingDto> bookingDtos =
            new List<BookingDto>();

        foreach (StudySession session in sessions)
        {
            // Studenter ser bare sine egne bookinger, admin ser alle.
            if (!CanManage(
                session,
                currentUserId,
                isAdmin))
            {
                continue;
            }

            bookingDtos.Add(
                ToDto(
                    session,
                    currentUserId,
                    isAdmin
                )
            );
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
            || !CanManage(
                session,
                currentUserId,
                isAdmin))
        {
            return null;
        }

        return ToDto(
            session,
            currentUserId,
            isAdmin
        );
    }

    public async Task<BookingDto> CreateAsync(
        CreateBookingDto bookingDto,
        string currentUserId)
    {
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

        return ToDto(
            createdSession,
            currentUserId,
            false
        );
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

        if (!CanManage(
            session,
            currentUserId,
            isAdmin))
        {
            throw new UnauthorizedAccessException(
                "You can only edit your own bookings."
            );
        }

        await ValidateAsync(
            bookingDto.RoomId,
            bookingDto.StartTime,
            bookingDto.EndTime,
            id
        );

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

        if (!CanManage(
            session,
            currentUserId,
            isAdmin))
        {
            throw new UnauthorizedAccessException(
                "You can only delete your own bookings."
            );
        }

        await _bookingRepository.DeleteAsync(session);

        return true;
    }

    private async Task ValidateAsync(
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
    }

    private bool CanManage(
        StudySession session,
        string currentUserId,
        bool isAdmin)
    {
        return isAdmin
            || session.OrganizerUserId == currentUserId;
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
                session.Room?.Building ?? string.Empty,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            OrganizerName =
                session.Organizer?.FullName ?? "Unknown",
            CanManage =
                CanManage(
                    session,
                    currentUserId,
                    isAdmin
                )
        };
    }
}