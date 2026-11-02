using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Api.DTOs;

public class UpdateBookingDto
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