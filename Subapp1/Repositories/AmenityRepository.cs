using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public class AmenityRepository(StudyRoomDbContext context) : IAmenityRepository
{
    public Task<List<Amenity>> GetAllAsync() => context.Amenities.AsNoTracking()
        .OrderBy(a => a.Name).ToListAsync();

    public Task<Amenity?> GetByIdAsync(int id) => context.Amenities
        .FirstOrDefaultAsync(a => a.Id == id);

    public Task<bool> NameExistsAsync(string name, int? exceptId = null) =>
        context.Amenities.AnyAsync(a => a.Name.ToLower() == name.ToLower() && a.Id != exceptId);

    public async Task AddAsync(Amenity amenity)
    {
        context.Amenities.Add(amenity);
        await context.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => context.SaveChangesAsync();

    public async Task DeleteAsync(Amenity amenity)
    {
        context.Amenities.Remove(amenity);
        await context.SaveChangesAsync();
    }
}
