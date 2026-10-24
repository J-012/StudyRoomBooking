using StudyRoomBooking.Models;
using StudyRoomBooking.Repositories;

namespace StudyRoomBooking.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;

    // Konstruktør for BookingService
    public BookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
    }
    // Valider en booking
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
}
