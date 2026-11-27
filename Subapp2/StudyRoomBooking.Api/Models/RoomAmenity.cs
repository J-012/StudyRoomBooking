namespace StudyRoomBooking.Api.Models;

// Join table between Room and Amenity. Key: (RoomId, AmenityId).
public class RoomAmenity
{
    public int RoomId { get; set; }
    public Room? Room { get; set; }

    public int AmenityId { get; set; }
    public Amenity? Amenity { get; set; }
}
