using System.ComponentModel.DataAnnotations;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.ViewModels.Admin;

public class AdminRoomsViewModel
{
    [StringLength(100)]
    public string? SearchTerm { get; set; }
    public string? Building { get; set; }

    [Range(1, 100, ErrorMessage = "Minimum capacity must be between 1 and 100.")]
    public int? MinCapacity { get; set; }

    public int? AmenityId { get; set; }
    public int Show { get; set; } = 9;
    public int TotalCount { get; set; }
    public List<Building> Buildings { get; set; } = new();
    public List<Room> Rooms { get; set; } = new();
    public List<Amenity> Amenities { get; set; } = new();
}
