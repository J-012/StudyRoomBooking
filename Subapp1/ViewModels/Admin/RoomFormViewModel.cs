using System.ComponentModel.DataAnnotations;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.ViewModels.Admin;

public class RoomFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string RoomNumber { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Select a building.")]
    public int BuildingId { get; set; }

    [Range(1, 100)]
    public int Capacity { get; set; }

    public List<int> AmenityIds { get; set; } = new();
    public List<Building> Buildings { get; set; } = new();
    public List<Amenity> Amenities { get; set; } = new();
}
