using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Repositories;

public interface IBuildingRepository
{
    Task<List<Building>> GetAllAsync();

    Task<Building?> GetByIdAsync(int id);

    Task<bool> ExistsByNameAsync(
        string name,
        int? excludedId = null
    );

    Task<Building> AddAsync(Building building);

    Task SaveChangesAsync();

    Task DeleteAsync(Building building);
}