using StudyRoomBooking.Models;
using StudyRoomBooking.Repositories;

namespace StudyRoomBooking.Services;

public class AmenityService(IAmenityRepository repository) : IAmenityService
{
    public Task<List<Amenity>> GetAllAsync() => repository.GetAllAsync();
    public Task<Amenity?> GetByIdAsync(int id) => repository.GetByIdAsync(id);

    public async Task<string?> CreateAsync(Amenity amenity)
    {
        amenity.Name = amenity.Name.Trim();
        amenity.Description = amenity.Description?.Trim();
        if (string.IsNullOrWhiteSpace(amenity.Name)) return "Enter an amenity name.";
        if (await repository.NameExistsAsync(amenity.Name)) return "An amenity with this name already exists.";
        await repository.AddAsync(amenity);
        return null;
    }

    public async Task<string?> UpdateAsync(int id, Amenity input)
    {
        var amenity = await repository.GetByIdAsync(id);
        if (amenity is null) return "Amenity not found.";
        var name = input.Name.Trim();
        if (string.IsNullOrWhiteSpace(name)) return "Enter an amenity name.";
        if (await repository.NameExistsAsync(name, id)) return "An amenity with this name already exists.";
        amenity.Name = name;
        amenity.Description = input.Description?.Trim();
        await repository.SaveChangesAsync();
        return null;
    }

    public async Task<string?> DeleteAsync(int id)
    {
        var amenity = await repository.GetByIdAsync(id);
        if (amenity is null) return "Amenity not found.";
        await repository.DeleteAsync(amenity);
        return null;
    }
}
