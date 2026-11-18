using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Api.DAL;
using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Repositories;

public class AmenityRepository : IAmenityRepository
{
    private readonly StudyRoomBookingDbContext _context;

    public AmenityRepository(
        StudyRoomBookingDbContext context)
    {
        _context = context;
    }

    public async Task<List<Amenity>> GetAllAsync()
    {
        return await _context.Amenities
            .AsNoTracking()
            .OrderBy(amenity => amenity.Name)
            .ToListAsync();
    }

    public async Task<Amenity?> GetByIdAsync(int id)
    {
        return await _context.Amenities
            .FirstOrDefaultAsync(
                amenity => amenity.Id == id
            );
    }

    public async Task<List<Amenity>> GetByIdsAsync(
        List<int> ids)
    {
        return await _context.Amenities
            .Where(amenity => ids.Contains(amenity.Id))
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        int? excludedId = null)
    {
        return await _context.Amenities
            .AnyAsync(amenity =>
                amenity.Name.ToLower() == name.ToLower()
                &&
                (!excludedId.HasValue
                    || amenity.Id != excludedId.Value)
            );
    }

    public async Task<Amenity> AddAsync(
        Amenity amenity)
    {
        await _context.Amenities.AddAsync(amenity);

        await _context.SaveChangesAsync();

        return amenity;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        Amenity amenity)
    {
        _context.Amenities.Remove(amenity);

        await _context.SaveChangesAsync();
    }
}