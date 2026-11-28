using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.DTOs;

public static class DtoMappings
{
    public static AmenityDto ToDto(this Amenity amenity) => new()
    {
        Id = amenity.Id,
        Name = amenity.Name,
        Description = amenity.Description
    };

    public static BuildingDto ToDto(this Building building) => new()
    {
        Id = building.Id,
        Name = building.Name,
        Address = building.Address
    };

    // Expects Building and RoomAmenities.Amenity to be loaded 
    public static RoomDto ToDto(this Room room) => new()
    {
        Id = room.Id,
        RoomNumber = room.RoomNumber,
        BuildingId = room.BuildingId,
        Building = room.Building?.Name ?? string.Empty,
        Capacity = room.Capacity,
        Amenities = room.RoomAmenities
            .Where(roomAmenity => roomAmenity.Amenity != null)
            .Select(roomAmenity => roomAmenity.Amenity!.ToDto())
            .ToList()
    };
}
