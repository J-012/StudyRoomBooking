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

    public Task<List<Building>> GetAllAsync() =>
        _context.Buildings
            .AsNoTracking()
            .OrderBy(building => building.Name)
            .ToListAsync();

    public Task<Building?> GetByIdAsync(int id) =>
        _context.Buildings.FirstOrDefaultAsync(building => building.Id == id);

    public Task<bool> ExistsByNameAsync(string name, int? excludedId = null) =>
        _context.Buildings.AnyAsync(building =>
            building.Name.ToLower() == name.ToLower()
            && (excludedId == null || building.Id != excludedId));

    public Task<bool> HasRoomsAsync(int buildingId) =>
        _context.Rooms.AnyAsync(room => room.BuildingId == buildingId);

    public async Task<Building> AddAsync(Building building)
    {
        _context.Buildings.Add(building);
        await _context.SaveChangesAsync();

        return building;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public async Task DeleteAsync(Building building)
    {
        _context.Buildings.Remove(building);
        await _context.SaveChangesAsync();
    }
}
