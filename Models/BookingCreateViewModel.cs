namespace StudyRoomBooking.Models;

public class BookingCreateViewModel
{
    public Booking Booking { get; set; } = new();

    public List<Room> Rooms { get; set; } = new();

    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public TimeOnly StartClock { get; set; } = TimeOnly.FromDateTime(DateTime.Now);

    public TimeOnly EndClock { get; set; } = TimeOnly.FromDateTime(DateTime.Now.AddHours(1));
}