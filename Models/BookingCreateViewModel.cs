namespace StudyRoomBooking.Models;

// ViewModel er for å samle både Booking og tilgjengelige rom for oppretting av en ny booking.
public class BookingCreateViewModel
{
    public Booking Booking { get; set; } = new();

    public List<Room> Rooms { get; set; } = new();
}
