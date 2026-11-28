using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public interface IBuildingRepository
{
    Task<List<Building>> GetAllAsync();
    Task<Building?> GetByIdAsync(int id);
    Task<bool> NameExistsAsync(string name, int? exceptId = null);
    Task<bool> HasRoomsAsync(int id);
    Task AddAsync(Building building);
    Task SaveChangesAsync();
    Task DeleteAsync(Building building);
}
