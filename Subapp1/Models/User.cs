using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models;

public class User
{
    [Key]
    [StringLength(50)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    // Use a UserRole enum instead of a string to make roles easier to manage and avoid typos.
    [Required]
    public UserRole Role { get; set; } = UserRole.Student;

    public List<Booking> Bookings { get; set; } = new();

    public Participant? Participant { get; set; }
}
