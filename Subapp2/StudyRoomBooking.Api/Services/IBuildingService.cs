using StudyRoomBooking.Api.DTOs;

namespace StudyRoomBooking.Api.Services;

public interface IBuildingService
{
    Task<List<BuildingDto>> GetAllAsync();

    Task<BuildingDto?> GetByIdAsync(int id);

    Task<BuildingDto> CreateAsync(
        CreateBuildingDto buildingDto
    );

    Task<bool> UpdateAsync(
        int id,
        UpdateBuildingDto buildingDto
    );

    Task<bool> DeleteAsync(int id);
}