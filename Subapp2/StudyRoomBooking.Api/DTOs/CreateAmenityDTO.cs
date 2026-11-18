using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Api.DTOs;

public class CreateAmenityDto
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }
}