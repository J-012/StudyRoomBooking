using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Repositories;

public interface IAmenityRepository
{
    Task<List<Amenity>> GetAllAsync();

    Task<Amenity?> GetByIdAsync(int id);

    Task<List<Amenity>> GetByIdsAsync(List<int> ids);

    Task<bool> ExistsByNameAsync(string name, int? excludedId = null);

    Task<Amenity> AddAsync(Amenity amenity);

    Task SaveChangesAsync();

    Task DeleteAsync(Amenity amenity);
}
