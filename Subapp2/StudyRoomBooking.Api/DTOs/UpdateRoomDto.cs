using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Api.DTOs;

public class UpdateRoomDto
{
    [Required]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Building { get; set; } = string.Empty;

    [Range(1, 100)]
    public int Capacity { get; set; }
}