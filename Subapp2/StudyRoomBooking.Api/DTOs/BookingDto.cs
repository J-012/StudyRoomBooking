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
}