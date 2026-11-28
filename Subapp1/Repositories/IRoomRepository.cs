using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public interface IRoomRepository
{
    // Get all rooms
    Task<List<Room>> GetAllAsync();

    // Get a specific room by ID.
    Task<Room?> GetByIdAsync(int id);

    // Check whether a room exists by ID.
    Task<bool> ExistsAsync(int id);

    // Add a new room.
    Task AddAsync(Room room);

    // Update an existing room.
    Task UpdateAsync(Room room);

    // Delete an existing room.
    Task DeleteAsync(Room room);

    //Search and filter rooms
    Task<List<Room>> SearchAndFilterAsync(
        string? searchTerm,
        string? building,
        int? minCapacity,
        int? amenityId);

    Task<List<Room>> GetRoomsWithBookingsAsync(
        DateTime startTime,
        DateTime endTime);

    Task<List<Room>> SearchWithBookingsAsync(
        string? searchTerm,
        string? building,
        int? minCapacity,
        DateTime startTime,
        DateTime endTime);

    Task<bool> BuildingExistsAsync(int buildingId);
    Task<bool> HasBookingsAsync(int roomId);
    Task<List<int>> GetValidAmenityIdsAsync(List<int> amenityIds);
    Task SaveWithAmenitiesAsync(Room room, List<int> amenityIds);
}
