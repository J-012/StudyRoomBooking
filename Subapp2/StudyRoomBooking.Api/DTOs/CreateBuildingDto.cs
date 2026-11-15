using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Api.DTOs;

public class CreateBuildingDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Address { get; set; }
}