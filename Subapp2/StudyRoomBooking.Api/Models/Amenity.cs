using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Api.Models;

// Equipment a room can have whiteboard, monitor, etc
public class Amenity
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    public List<RoomAmenity> RoomAmenities { get; set; } = new();
}
