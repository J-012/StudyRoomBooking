using StudyRoomBooking.Models;

namespace StudyRoomBooking.Services;

public interface IAmenityService
{
    Task<List<Amenity>> GetAllAsync();
    Task<Amenity?> GetByIdAsync(int id);
    Task<string?> CreateAsync(Amenity amenity);
    Task<string?> UpdateAsync(int id, Amenity input);
    Task<string?> DeleteAsync(int id);
}
