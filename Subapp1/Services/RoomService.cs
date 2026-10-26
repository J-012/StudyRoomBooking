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

    public Task<List<Room>> SearchAndFilterAsync(
    string? searchTerm,
    string? building,
    int? minCapacity)
{
    return _roomRepository.SearchAndFilterAsync(
        searchTerm,
        building,
        minCapacity);
}
}