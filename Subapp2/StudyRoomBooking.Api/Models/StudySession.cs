using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyRoomBooking.Api.Models;

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

    // One room can have many study sessions.
    public int RoomId { get; set; }

    [ForeignKey(nameof(RoomId))]
    public Room? Room { get; set; }

    // The logged-in user who created the booking.
    // Nullable so older database rows still work after the migration.
    public string? OrganizerUserId { get; set; }

    [ForeignKey(nameof(OrganizerUserId))]
    public ApplicationUser? Organizer { get; set; }

    // Many-to-many participation through SessionParticipant.
    public List<SessionParticipant> Participants { get; set; } = new();
}