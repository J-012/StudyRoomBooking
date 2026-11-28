using Moq;
using StudyRoomBooking.Models;
using StudyRoomBooking.Services;
using StudyRoomBooking.Repositories;
using Xunit;

namespace Subapp1.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookings = new();
    private readonly Mock<IRoomRepository> _rooms = new();
    private readonly Mock <IParticipantRepository> _participants = new();

    private BookingService CreateService() => new(_bookings.Object, _rooms.Object, _participants.Object);

    private static Booking FutureBooking(int durationMinutes = 60) => new()
    {
          RoomId = 1,
        UserId = "owner",
        StartTime = DateTime.Now.AddDays(2),
        EndTime = DateTime.Now.AddDays(2).AddMinutes(durationMinutes)
    };

    private void ValidRoomAndNoConflict()
    {
        _rooms.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
        _bookings.Setup(x => x.HasConflictAsync(It.IsAny<Booking>(), It.IsAny<int?>()))
            .ReturnsAsync(false);
    }

    // POSITIVE TESTS (6)
    [Fact]
    public async Task ValidBooking_IsAccepted()
    {
        ValidRoomAndNoConflict();
        var result = await CreateService().ValidateAsync(FutureBooking(), null);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task BookingExactly15Minutes_IsAccepted()
    {
        ValidRoomAndNoConflict();
        var booking = FutureBooking();
        booking.EndTime = booking.StartTime.AddMinutes(15);
        Assert.True((await CreateService().ValidateAsync(booking, null)).IsValid);
    }

    [Fact]
    public async Task BookingExactly180Minutes_IsAccepted()
    {
        ValidRoomAndNoConflict();
        var booking = FutureBooking();
        booking.EndTime = booking.StartTime.AddMinutes(180);
        Assert.True((await CreateService().ValidateAsync(booking, null)).IsValid);
    }

    [Fact]
    public async Task BookingWithoutConflict_IsAccepted()
    {
        ValidRoomAndNoConflict();
        var booking = FutureBooking();
        var result = await CreateService().ValidateAsync(booking, null);
        Assert.True(result.IsValid);
        _bookings.Verify(x => x.HasConflictAsync(booking, null), Times.Once);
    }

    [Fact]
    public async Task JoiningAvailableSession_Succeeds()
    {
        var booking = FutureBooking();
        booking.BookingId = 42;
        booking.Room = new Room { Id = 1, Capacity = 4 };
        _bookings.Setup(x => x.GetWithParticipantsAsync(42)).ReturnsAsync(booking);
        _participants.Setup(x => x.GetByUserIdAsync("student")).ReturnsAsync(new Participant { ParticipantId = 7, UserId = "student" });
        var result = await CreateService().JoinAsync(42, "student");
        Assert.True(result.IsSuccess);
        _participants.Verify(x => x.AddToBookingAsync(It.Is<BookingParticipant>(p => p.BookingId == 42 && p.ParticipantId == 7)), Times.Once);
    }

    [Fact]
    public async Task LeavingJoinedSession_Succeeds()
    {
        var booking = FutureBooking();
        booking.BookingId = 42;
        var membership = new BookingParticipant { BookingId = 42, ParticipantId = 7, Participant = new Participant { ParticipantId = 7, UserId = "student" } };
        booking.Participants.Add(membership);
        _bookings.Setup(x => x.GetWithParticipantsAsync(42)).ReturnsAsync(booking);
        var result = await CreateService().LeaveAsync(42, "student");
        Assert.True(result.IsSuccess);
        _participants.Verify(x => x.RemoveFromBookingAsync(membership), Times.Once);
    }

    // NEGATIVE TESTS (6)
    [Fact]
    public async Task InvalidRoom_IsRejected()
    {
        ValidRoomAndNoConflict();
        var booking = FutureBooking();
        booking.RoomId = -1;
        var result = await CreateService().ValidateAsync(booking, null);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Key == nameof(Booking.RoomId));
    }

    [Fact]
    public async Task EndBeforeStart_IsRejected()
    {
        ValidRoomAndNoConflict();
        var booking = FutureBooking();
        booking.EndTime = booking.StartTime.AddMinutes(-1);
        var result = await CreateService().ValidateAsync(booking, null);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Key == "EndClock");
    }

    [Fact]
    public async Task BookingShorterThan15Minutes_IsRejected()
    {
        ValidRoomAndNoConflict();
        var booking = FutureBooking();
        booking.EndTime = booking.StartTime.AddMinutes(14);
        var result = await CreateService().ValidateAsync(booking, null);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message.Contains("at least 15 minutes"));
    }

    [Fact]
    public async Task BookingLongerThan180Minutes_IsRejected()
    {
        ValidRoomAndNoConflict();
        var booking = FutureBooking();
        booking.EndTime = booking.StartTime.AddMinutes(181);
        var result = await CreateService().ValidateAsync(booking, null);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message.Contains("three hours"));
    }

    [Fact]
    public async Task BookingInPast_IsRejected()
    {
        ValidRoomAndNoConflict();
        var booking = FutureBooking();
        booking.StartTime = DateTime.Now.AddDays(-2);
        booking.EndTime = booking.StartTime.AddHours(1);
        var result = await CreateService().ValidateAsync(booking, null);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Key == "StartClock");
    }

    [Fact]
    public async Task OverlappingBooking_IsRejected()
    {
        ValidRoomAndNoConflict();
        _bookings.Setup(x => x.HasConflictAsync(It.IsAny<Booking>(), It.IsAny<int?>())).ReturnsAsync(true);
        var result = await CreateService().ValidateAsync(FutureBooking(), null);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message.Contains("already booked"));
    }
}