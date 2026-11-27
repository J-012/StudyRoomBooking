namespace StudyRoomBooking.Api.DTOs;

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

    public bool CanManage { get; set; }

    // Number of seats in the room.
    public int Capacity { get; set; }

    // The organizer plus everyone who has joined.
    public int AttendeeCount { get; set; }

    // True if the current user created the session.
    public bool IsOrganizer { get; set; }

    // True if the current user has joined someone else's session.
    public bool IsParticipant { get; set; }
}
