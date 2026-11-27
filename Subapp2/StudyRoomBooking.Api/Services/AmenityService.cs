using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Repositories;

namespace StudyRoomBooking.Api.Services;

public class AmenityService : IAmenityService
{
    private readonly IAmenityRepository _amenityRepository;

    public AmenityService(IAmenityRepository amenityRepository)
    {
        _amenityRepository = amenityRepository;
    }

    public async Task<List<AmenityDto>> GetAllAsync()
    {
        List<Amenity> amenities = await _amenityRepository.GetAllAsync();

        return amenities.Select(amenity => amenity.ToDto()).ToList();
    }

    public async Task<AmenityDto?> GetByIdAsync(int id)
    {
        Amenity? amenity = await _amenityRepository.GetByIdAsync(id);

        return amenity?.ToDto();
    }

    public async Task<AmenityDto> CreateAsync(AmenityRequestDto amenityDto)
    {
        Amenity amenity = new Amenity
        {
            Name = await ValidateNameAsync(amenityDto.Name, null),
            Description = amenityDto.Description?.Trim()
        };

        Amenity createdAmenity = await _amenityRepository.AddAsync(amenity);

        return createdAmenity.ToDto();
    }

    public async Task<bool> UpdateAsync(int id, AmenityRequestDto amenityDto)
    {
        Amenity? amenity = await _amenityRepository.GetByIdAsync(id);

        if (amenity == null)
        {
            return false;
        }

        amenity.Name = await ValidateNameAsync(amenityDto.Name, id);
        amenity.Description = amenityDto.Description?.Trim();

        await _amenityRepository.SaveChangesAsync();

        return true;
    }

    // RoomAmenity rows are removed by cascade delete.
    public async Task<bool> DeleteAsync(int id)
    {
        Amenity? amenity = await _amenityRepository.GetByIdAsync(id);

        if (amenity == null)
        {
            return false;
        }

        await _amenityRepository.DeleteAsync(amenity);

        return true;
    }

    // Returns the trimmed name, or throws if another amenity already uses it.
    private async Task<string> ValidateNameAsync(string name, int? excludedId)
    {
        string trimmedName = name.Trim();

        if (await _amenityRepository.ExistsByNameAsync(trimmedName, excludedId))
        {
            throw new ArgumentException("An amenity with this name already exists.");
        }

        return trimmedName;
    }
}
