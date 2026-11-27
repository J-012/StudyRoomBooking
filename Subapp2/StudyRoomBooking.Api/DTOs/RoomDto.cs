using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Api.DTOs;

// Sent to the client.
public class RoomDto
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int BuildingId { get; set; }

    // Name of the building, so the client does not need a second request.
    public string Building { get; set; } = string.Empty;

    public int Capacity { get; set; }
    public List<AmenityDto> Amenities { get; set; } = new();
}

// Received from the client. Used for both create (POST) and update (PUT).
public class RoomRequestDto
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
