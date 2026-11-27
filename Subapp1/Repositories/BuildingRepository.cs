using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public class BuildingRepository(StudyRoomDbContext context) : IBuildingRepository
{
    public Task<List<Building>> GetAllAsync() => context.Buildings.AsNoTracking()
        .OrderBy(b => b.Name).ToListAsync();

    public Task<Building?> GetByIdAsync(int id) => context.Buildings
        .FirstOrDefaultAsync(b => b.Id == id);

    public Task<bool> NameExistsAsync(string name, int? exceptId = null) =>
        context.Buildings.AnyAsync(b => b.Name.ToLower() == name.ToLower() && b.Id != exceptId);

    public Task<bool> HasRoomsAsync(int id) => context.Rooms.AnyAsync(r => r.BuildingId == id);

    public async Task AddAsync(Building building)
    {
        context.Buildings.Add(building);
        await context.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => context.SaveChangesAsync();

    public async Task DeleteAsync(Building building)
    {
        context.Buildings.Remove(building);
        await context.SaveChangesAsync();
    }
}
