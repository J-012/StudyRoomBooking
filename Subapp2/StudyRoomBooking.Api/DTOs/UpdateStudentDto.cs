using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Api.DTOs;

public class UpdateStudentDto
{
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
}