
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

    // Include rooms so they can be displayed in the view's dropdown.
    public List<Room> Rooms { get; set; } = new();

    public DateOnly StartDate { get; set; } =
        DateOnly.FromDateTime(DateTime.Now);

    public TimeOnly StartClock { get; set; } =
        TimeOnly.FromDateTime(DateTime.Now);

    public TimeOnly EndClock { get; set; } =
        TimeOnly.FromDateTime(DateTime.Now.AddHours(1));
}
