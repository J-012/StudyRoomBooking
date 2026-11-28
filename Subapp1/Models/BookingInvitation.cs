namespace StudyRoomBooking.Models;

public class BookingInvitation
{
    public int BookingId { get; set; }

    public Booking Booking { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}
