using StudyRoomBooking.Api.DTOs;

namespace StudyRoomBooking.Api.Services;

// Update/Delete return false when the building does not exist
// Broken rules throw ArgumentException
public interface IBuildingService
{
    Task<List<BuildingDto>> GetAllAsync();

    Task<BuildingDto?> GetByIdAsync(int id);

    Task<BuildingDto> CreateAsync(BuildingRequestDto buildingDto);

    Task<bool> UpdateAsync(int id, BuildingRequestDto buildingDto);

    Task<bool> DeleteAsync(int id);
}
