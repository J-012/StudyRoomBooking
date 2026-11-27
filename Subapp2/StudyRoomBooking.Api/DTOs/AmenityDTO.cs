using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Api.DTOs;

// Sent to the client.
public class AmenityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

// Received from the client. Used for both create (POST) and update (PUT).
public class AmenityRequestDto
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }
}
