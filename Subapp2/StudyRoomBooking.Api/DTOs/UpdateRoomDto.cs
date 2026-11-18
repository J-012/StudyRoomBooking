using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Api.DTOs;

public class UpdateRoomDto
{
    [Required]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int BuildingId { get; set; }

    [Range(1, 100)]
    public int Capacity { get; set; }

    public List<int> AmenityIds { get; set; } = new();
}