using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models;

public class Booking
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Topic { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string StudentName { get; set; } = string.Empty;

    [Required]
    public int RoomId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }
}
