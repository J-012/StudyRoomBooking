using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Repositories;

namespace StudyRoomBooking.Api.Services;

public class BuildingService : IBuildingService
{
    private readonly IBuildingRepository _buildingRepository;

    public BuildingService(
        IBuildingRepository buildingRepository)
    {
        _buildingRepository = buildingRepository;
    }

    public async Task<List<BuildingDto>> GetAllAsync()
    {
        List<Building> buildings =
            await _buildingRepository.GetAllAsync();

        List<BuildingDto> buildingDtos =
            new List<BuildingDto>();

        foreach (Building building in buildings)
        {
            buildingDtos.Add(ToDto(building));
        }

        return buildingDtos;
    }

    public async Task<BuildingDto?> GetByIdAsync(int id)
    {
        Building? building =
            await _buildingRepository.GetByIdAsync(id);

        if (building == null)
        {
            return null;
        }

        return ToDto(building);
    }

    public async Task<BuildingDto> CreateAsync(
        CreateBuildingDto buildingDto)
    {
        string name = buildingDto.Name.Trim();

        bool alreadyExists =
            await _buildingRepository.ExistsByNameAsync(
                name
            );

        if (alreadyExists)
        {
            throw new ArgumentException(
                "A building with this name already exists."
            );
        }

        Building building = new Building
        {
            Name = name,
            Address = buildingDto.Address?.Trim()
        };

        Building createdBuilding =
            await _buildingRepository.AddAsync(building);

        return ToDto(createdBuilding);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateBuildingDto buildingDto)
    {
        Building? building =
            await _buildingRepository.GetByIdAsync(id);

        if (building == null)
        {
            return false;
        }

        string name = buildingDto.Name.Trim();

        bool alreadyExists =
            await _buildingRepository.ExistsByNameAsync(
                name,
                id
            );

        if (alreadyExists)
        {
            throw new ArgumentException(
                "A building with this name already exists."
            );
        }

        building.Name = name;
        building.Address =
            buildingDto.Address?.Trim();

        await _buildingRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Building? building =
            await _buildingRepository.GetByIdAsync(id);

        if (building == null)
        {
            return false;
        }

        await _buildingRepository.DeleteAsync(building);

        return true;
    }

    private BuildingDto ToDto(Building building)
    {
        return new BuildingDto
        {
            Id = building.Id,
            Name = building.Name,
            Address = building.Address
        };
    }
}