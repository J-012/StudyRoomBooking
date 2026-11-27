namespace StudyRoomBooking.Api.DTOs;

public class RoomDto
{
    public int Id { get; set; }

    public string RoomNumber { get; set; } = string.Empty;

    public int BuildingId { get; set; }

    public string Building { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public List<AmenityDto> Amenities { get; set; } = new();
}