namespace StudyRoomBooking.Api.DTOs;

public class BuildingDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Address { get; set; }
}