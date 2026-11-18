using StudyRoomBooking.Api.DTOs;

namespace StudyRoomBooking.Api.Services;

public interface IAmenityService
{
    Task<List<AmenityDto>> GetAllAsync();

    Task<AmenityDto?> GetByIdAsync(int id);

    Task<AmenityDto> CreateAsync(
        CreateAmenityDto amenityDto
    );

    Task<bool> UpdateAsync(
        int id,
        UpdateAmenityDto amenityDto
    );

    Task<bool> DeleteAsync(int id);
}