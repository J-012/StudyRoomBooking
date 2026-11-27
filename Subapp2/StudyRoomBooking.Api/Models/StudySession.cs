using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyRoomBooking.Api.Models;

// A booking of a room. Called "booking" in the API and the client.
public class StudySession
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Topic { get; set; } = string.Empty;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public int RoomId { get; set; }

    [ForeignKey(nameof(RoomId))]
    public Room? Room { get; set; }

    // The user who created the booking. Nullable so that deleting a user
    public string? OrganizerUserId { get; set; }

    [ForeignKey(nameof(OrganizerUserId))]
    public ApplicationUser? Organizer { get; set; }

    // Other users who have joined the session.
    public List<SessionParticipant> Participants { get; set; } = new();
}
