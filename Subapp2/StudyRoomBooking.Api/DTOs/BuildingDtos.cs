using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Api.DTOs;

// Sent to the client.
public class BuildingDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
}

// Received from the client. Used for both create (POST) and update (PUT).
public class BuildingRequestDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Address { get; set; }
}