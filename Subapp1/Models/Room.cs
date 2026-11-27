using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models;

public class Room
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string RoomNumber { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Select a building.")]
    public int BuildingId { get; set; }
    public Building? Building { get; set; }

    [Required]
    [Range(1, 100)]
    public int Capacity { get; set; }

    public List<Booking> Bookings { get; set; } = new();
    public List<RoomAmenity> RoomAmenities { get; set; } = new();
}
