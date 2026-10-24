
using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models;

public class BookingCreateViewModel
{
    public int BookingId { get; set; }

    [Required]
    [StringLength(100)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Topic { get; set; } = string.Empty;

    [Required]
    public int RoomId { get; set; }

    // Måtte inkludere listen over rom for å kunne vise dem i dropdown i viewet.
    public List<Room> Rooms { get; set; } = new();

    public DateOnly StartDate { get; set; } =
        DateOnly.FromDateTime(DateTime.Now);

    public TimeOnly StartClock { get; set; } =
        TimeOnly.FromDateTime(DateTime.Now);

    public TimeOnly EndClock { get; set; } =
        TimeOnly.FromDateTime(DateTime.Now.AddHours(1));
}
