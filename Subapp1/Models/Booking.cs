using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models;

public class Booking
{
    public int BookingId { get; set; }

    [StringLength(20)]
    public string? CourseCode { get; set; }

    public Course? Course { get; set; }

    [Required]
    [StringLength(200)]
    public string Topic { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public int RoomId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public User? User { get; set; }

    public Room? Room { get; set; }


}
