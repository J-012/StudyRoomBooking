namespace StudyRoomBooking.Models;

public class BookingParticipant
{
    public int BookingId { get; set; }

    public Booking Booking { get; set; } = null!;

    public int ParticipantId { get; set; }

    public Participant Participant { get; set; } = null!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}