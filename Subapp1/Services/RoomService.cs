
using StudyRoomBooking.Models;
using StudyRoomBooking.Repositories;

namespace StudyRoomBooking.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public Task<List<Room>> GetAllAsync()
    {
        return _roomRepository.GetAllAsync();
    }

    public Task<Room?> GetByIdAsync(int id)
    {
        return _roomRepository.GetByIdAsync(id);
    }

    public Task AddAsync(Room room)
    {
        return _roomRepository.AddAsync(room);
    }

    public Task UpdateAsync(Room room)
    {
        return _roomRepository.UpdateAsync(room);
    }

    public Task DeleteAsync(Room room)
    {
        return _roomRepository.DeleteAsync(room);
    }

    public async Task<string?> SaveAsync(Room room, List<int> amenityIds)
    {
        if (!await _roomRepository.BuildingExistsAsync(room.BuildingId))
            return "Select an existing building.";

        // Normalize posted IDs before confirming every selected amenity still exists.
        var selectedIds = amenityIds.Distinct().ToList();
        var validIds = await _roomRepository.GetValidAmenityIdsAsync(selectedIds);
        if (validIds.Count != selectedIds.Count)
            return "One or more selected amenities no longer exist.";

        await _roomRepository.SaveWithAmenitiesAsync(room, selectedIds);
        return null;
    }

    public async Task<string?> DeleteForAdminAsync(int id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        if (room is null) return "Room not found.";
        if (await _roomRepository.HasBookingsAsync(id))
            return "This room has bookings. Remove those bookings before deleting the room.";

        await _roomRepository.DeleteAsync(room);
        return null;
    }

    public Task<List<Room>> SearchAndFilterAsync(
        string? searchTerm,
        string? building,
        int? minCapacity,
        int? amenityId)
    {
        return _roomRepository.SearchAndFilterAsync(
            searchTerm,
            building,
            minCapacity,
            amenityId);
    }

    public async Task<List<Room>> SearchAndFilterByAvailabilityAsync(
        string? searchTerm,
        string? building,
        int? minCapacity,
        DateTime startTime,
        DateTime endTime,
        string status)
    {
        // Validate the selected time interval.
        if (startTime >= endTime)
        {
            throw new ArgumentException(
                "End time must be later than start time.");
        }

        // Fetch rooms matching the existing filters,
        // including bookings overlapping the selected interval.
        var rooms = await _roomRepository
            .SearchWithBookingsAsync(
                searchTerm,
                building,
                minCapacity,
                startTime,
                endTime);

        return status switch
        {
            "available" => rooms
                .Where(r => !r.Bookings.Any())
                .ToList(),

            "booked" => rooms
                .Where(r => r.Bookings.Any())
                .ToList(),

            "all" => rooms,

            _ => throw new ArgumentException(
                "Invalid room status.")
        };
    }
}
