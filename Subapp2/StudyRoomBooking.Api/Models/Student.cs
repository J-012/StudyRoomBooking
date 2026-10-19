using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Api.Models;

public class Student
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string StudentNumber { get; set; } = string.Empty;

    // Navigation property - en student kan delta i flere sesjoner
    public List<SessionParticipant> Participations { get; set; } = new();
}