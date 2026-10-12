using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models;

public class Room
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Building { get; set; } = string.Empty;

    [Required]
    [Range(1, 100)]
    public int Capacity { get; set; }
}
