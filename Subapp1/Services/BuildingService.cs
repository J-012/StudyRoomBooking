using StudyRoomBooking.Models;
using StudyRoomBooking.Repositories;

namespace StudyRoomBooking.Services;

public class BuildingService(IBuildingRepository repository) : IBuildingService
{
    public Task<List<Building>> GetAllAsync() => repository.GetAllAsync();
    public Task<Building?> GetByIdAsync(int id) => repository.GetByIdAsync(id);

    public async Task<string?> CreateAsync(Building building)
    {
        building.Name = building.Name.Trim();
        building.Address = building.Address?.Trim();
        if (string.IsNullOrWhiteSpace(building.Name)) return "Enter a building name.";
        if (await repository.NameExistsAsync(building.Name)) return "A building with this name already exists.";
        await repository.AddAsync(building);
        return null;
    }

    public async Task<string?> UpdateAsync(int id, Building input)
    {
        var building = await repository.GetByIdAsync(id);
        if (building is null) return "Building not found.";
        var name = input.Name.Trim();
        if (string.IsNullOrWhiteSpace(name)) return "Enter a building name.";
        if (await repository.NameExistsAsync(name, id)) return "A building with this name already exists.";
        building.Name = name;
        building.Address = input.Address?.Trim();
        await repository.SaveChangesAsync();
        return null;
    }

    public async Task<string?> DeleteAsync(int id)
    {
        var building = await repository.GetByIdAsync(id);
        if (building is null) return "Building not found.";
        if (await repository.HasRoomsAsync(id)) return "Move or delete this building's rooms before deleting it.";
        await repository.DeleteAsync(building);
        return null;
    }
}
