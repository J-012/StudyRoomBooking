using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Api.Models;

public class Room
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string RoomNumber { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int BuildingId { get; set; }
    public Building? Building { get; set; }

    [Range(1, 100)]
    public int Capacity { get; set; }

    public List<RoomAmenity> RoomAmenities { get; set; } = new();
}