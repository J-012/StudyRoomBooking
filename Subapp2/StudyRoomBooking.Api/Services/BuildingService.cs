using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Repositories;

namespace StudyRoomBooking.Api.Services;

public class BuildingService : IBuildingService
{
    private readonly IBuildingRepository _buildingRepository;

    public BuildingService(IBuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
    }

    public async Task<List<BuildingDto>> GetAllAsync()
    {
        List<Building> buildings = await _buildingRepository.GetAllAsync();

        return buildings.Select(building => building.ToDto()).ToList();
    }

    public async Task<BuildingDto?> GetByIdAsync(int id)
    {
        Building? building = await _buildingRepository.GetByIdAsync(id);

        return building?.ToDto();
    }

    public async Task<BuildingDto> CreateAsync(BuildingRequestDto buildingDto)
    {
        Building building = new Building
        {
            Name = await ValidateNameAsync(buildingDto.Name, null),
            Address = buildingDto.Address?.Trim()
        };

        Building createdBuilding = await _buildingRepository.AddAsync(building);

        return createdBuilding.ToDto();
    }

    public async Task<bool> UpdateAsync(int id, BuildingRequestDto buildingDto)
    {
        Building? building = await _buildingRepository.GetByIdAsync(id);

        if (building == null)
        {
            return false;
        }

        building.Name = await ValidateNameAsync(buildingDto.Name, id);
        building.Address = buildingDto.Address?.Trim();

        await _buildingRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Building? building = await _buildingRepository.GetByIdAsync(id);

        if (building == null)
        {
            return false;
        }

        // Rooms reference their building with DeleteBehavior.Restrict
        if (await _buildingRepository.HasRoomsAsync(id))
        {
            throw new ArgumentException("This building still has rooms. Move or delete the rooms first.");
        }

        await _buildingRepository.DeleteAsync(building);

        return true;
    }

    // Returns the trimmed name, or throws if another building already uses it.
    private async Task<string> ValidateNameAsync(string name, int? excludedId)
    {
        string trimmedName = name.Trim();

        if (await _buildingRepository.ExistsByNameAsync(trimmedName, excludedId))
        {
            throw new ArgumentException("A building with this name already exists.");
        }

        return trimmedName;
    }
}
