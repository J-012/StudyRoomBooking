using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models;

public class Course
{
    [Key]
    [Required]
    [StringLength(20)]
    public string CourseCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    // Ett emne kan ha flere bookinger
    public List<Booking> Bookings { get; set; } = new();
}
