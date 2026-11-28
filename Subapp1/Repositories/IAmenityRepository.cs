using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public interface IAmenityRepository
{
    Task<List<Amenity>> GetAllAsync();
    Task<Amenity?> GetByIdAsync(int id);
    Task<bool> NameExistsAsync(string name, int? exceptId = null);
    Task AddAsync(Amenity amenity);
    Task SaveChangesAsync();
    Task DeleteAsync(Amenity amenity);
}
