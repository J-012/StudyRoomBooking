using StudyRoomBooking.Models;

namespace StudyRoomBooking.Services;

public interface IRoomService
{
    Task<List<Room>> GetAllAsync();

    Task<Room?> GetByIdAsync(int id);

    Task AddAsync(Room room);

    Task UpdateAsync(Room room);

    Task DeleteAsync(Room room);
    Task<string?> SaveAsync(Room room, List<int> amenityIds);
    Task<string?> DeleteForAdminAsync(int id);
    Task<List<Room>> SearchAndFilterAsync(
        string? searchTerm,
        string? building,
        int? minCapacity,
        int? amenityId);

    Task<List<Room>> SearchAndFilterByAvailabilityAsync(
        string? searchTerm,
        string? building,
        int? minCapacity,
        DateTime startTime,
        DateTime endTime,
        string status);
}
