using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models;

public class Building
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Address { get; set; }

    public List<Room> Rooms { get; set; } = new();
}
