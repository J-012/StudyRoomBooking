using StudyRoomBooking.Models;

namespace StudyRoomBooking.ViewModels.Admin;

public class AmenityManagerViewModel
{
    public IReadOnlyList<Amenity> Amenities { get; set; } = Array.Empty<Amenity>();
    // When set, catalog actions return to the room currently being edited.
    public int? ReturnRoomId { get; set; }
}
