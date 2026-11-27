using StudyRoomBooking.Models;

namespace StudyRoomBooking.Services;

public interface IBuildingService
{
    Task<List<Building>> GetAllAsync();
    Task<Building?> GetByIdAsync(int id);
    Task<string?> CreateAsync(Building building);
    Task<string?> UpdateAsync(int id, Building input);
    Task<string?> DeleteAsync(int id);
}
