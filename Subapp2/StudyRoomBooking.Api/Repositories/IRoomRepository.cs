using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Repositories;

public interface IRoomRepository
{
    Task<List<Room>> GetAllAsync();

    Task<Room?> GetByIdAsync(int id);

    Task<Room> AddAsync(Room room);

    Task SaveChangesAsync();

    Task DeleteAsync(Room room);
}