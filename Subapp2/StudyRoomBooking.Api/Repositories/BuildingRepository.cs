using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Api.DAL;
using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Repositories;

public class BuildingRepository : IBuildingRepository
{
    private readonly StudyRoomBookingDbContext _context;

    public BuildingRepository(StudyRoomBookingDbContext context)
    {
        _context = context;
    }

    public async Task<List<Building>> GetAllAsync()
    {
        return await _context.Buildings
            .AsNoTracking()
            .OrderBy(building => building.Name)
            .ToListAsync();
    }

    public async Task<Building?> GetByIdAsync(int id)
    {
        return await _context.Buildings
            .FirstOrDefaultAsync(building => building.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        int? excludedId = null)
    {
        return await _context.Buildings
            .AnyAsync(building =>
                building.Name.ToLower() == name.ToLower()
                && (!excludedId.HasValue
                    || building.Id != excludedId.Value)
            );
    }

    public async Task<bool> HasRoomsAsync(int buildingId)
    {
        return await _context.Rooms
            .AnyAsync(room => room.BuildingId == buildingId);
    }

    public async Task<Building> AddAsync(Building building)
    {
        await _context.Buildings.AddAsync(building);

        await _context.SaveChangesAsync();

        return building;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Building building)
    {
        _context.Buildings.Remove(building);

        await _context.SaveChangesAsync();
    }
}
