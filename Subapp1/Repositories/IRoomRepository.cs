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
}
