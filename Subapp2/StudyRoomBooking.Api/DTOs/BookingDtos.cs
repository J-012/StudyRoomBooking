using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Api.DTOs;

// Sent to the client. Some fields depend on who is asking
// (CanManage, IsOrganizer, IsParticipant).
public class BookingDto
{
    public int Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;

    public int RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public string OrganizerName { get; set; } = string.Empty;

    // Number of seats in the room.
    public int Capacity { get; set; }

    // The organizer plus everyone who has joined.
    public int AttendeeCount { get; set; }

    // True if the current user may edit/delete (organizer or admin).
    public bool CanManage { get; set; }

    // True if the current user created the session.
    public bool IsOrganizer { get; set; }

    // True if the current user has joined someone else's session.
    public bool IsParticipant { get; set; }
}

// Received from the client. Used for both create (POST) and update (PUT).
// Time rules (duration, not in the past, no overlap) are checked in BookingService.
public class BookingRequestDto
{
    [Required]
    [StringLength(100)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Topic { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int RoomId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
