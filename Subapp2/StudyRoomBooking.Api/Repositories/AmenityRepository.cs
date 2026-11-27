using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Api.DAL;
using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Repositories;

public class AmenityRepository : IAmenityRepository
{
    private readonly StudyRoomBookingDbContext _context;

    public AmenityRepository(StudyRoomBookingDbContext context)
    {
        _context = context;
    }

    public Task<List<Amenity>> GetAllAsync() =>
        _context.Amenities
            .AsNoTracking()
            .OrderBy(amenity => amenity.Name)
            .ToListAsync();

    public Task<Amenity?> GetByIdAsync(int id) =>
        _context.Amenities.FirstOrDefaultAsync(amenity => amenity.Id == id);

    public Task<List<Amenity>> GetByIdsAsync(List<int> ids) =>
        _context.Amenities
            .Where(amenity => ids.Contains(amenity.Id))
            .ToListAsync();

    public Task<bool> ExistsByNameAsync(string name, int? excludedId = null) =>
        _context.Amenities.AnyAsync(amenity =>
            amenity.Name.ToLower() == name.ToLower()
            && (excludedId == null || amenity.Id != excludedId));

    public async Task<Amenity> AddAsync(Amenity amenity)
    {
        _context.Amenities.Add(amenity);
        await _context.SaveChangesAsync();

        return amenity;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public async Task DeleteAsync(Amenity amenity)
    {
        _context.Amenities.Remove(amenity);
        await _context.SaveChangesAsync();
    }
}
