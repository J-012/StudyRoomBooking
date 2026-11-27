using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models;

public class Participant
{
    public int ParticipantId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public User User { get; set; } = null!;

    public List<BookingParticipant> BookingParticipants { get; set; } = new();
}