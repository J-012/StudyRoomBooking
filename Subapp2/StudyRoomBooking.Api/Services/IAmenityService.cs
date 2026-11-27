using StudyRoomBooking.Api.DTOs;

namespace StudyRoomBooking.Api.Services;

// Update/Delete return false when the amenity does not exist 
// Broken rules throw ArgumentException
public interface IAmenityService
{
    Task<List<AmenityDto>> GetAllAsync();

    Task<AmenityDto?> GetByIdAsync(int id);

    Task<AmenityDto> CreateAsync(AmenityRequestDto amenityDto);

    Task<bool> UpdateAsync(int id, AmenityRequestDto amenityDto);

    Task<bool> DeleteAsync(int id);
}
