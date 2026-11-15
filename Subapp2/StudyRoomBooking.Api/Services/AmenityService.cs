using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Repositories;

namespace StudyRoomBooking.Api.Services;

public class AmenityService : IAmenityService
{
    private readonly IAmenityRepository _amenityRepository;

    public AmenityService(
        IAmenityRepository amenityRepository)
    {
        _amenityRepository = amenityRepository;
    }

    public async Task<List<AmenityDto>> GetAllAsync()
    {
        List<Amenity> amenities =
            await _amenityRepository.GetAllAsync();

        List<AmenityDto> amenityDtos =
            new List<AmenityDto>();

        foreach (Amenity amenity in amenities)
        {
            amenityDtos.Add(ToDto(amenity));
        }

        return amenityDtos;
    }

    public async Task<AmenityDto?> GetByIdAsync(
        int id)
    {
        Amenity? amenity =
            await _amenityRepository.GetByIdAsync(id);

        if (amenity == null)
        {
            return null;
        }

        return ToDto(amenity);
    }

    public async Task<AmenityDto> CreateAsync(
        CreateAmenityDto amenityDto)
    {
        string name = amenityDto.Name.Trim();

        bool alreadyExists =
            await _amenityRepository.ExistsByNameAsync(
                name
            );

        if (alreadyExists)
        {
            throw new ArgumentException(
                "An amenity with this name already exists."
            );
        }

        Amenity amenity = new Amenity
        {
            Name = name,
            Description =
                amenityDto.Description?.Trim()
        };

        Amenity createdAmenity =
            await _amenityRepository.AddAsync(
                amenity
            );

        return ToDto(createdAmenity);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateAmenityDto amenityDto)
    {
        Amenity? amenity =
            await _amenityRepository.GetByIdAsync(id);

        if (amenity == null)
        {
            return false;
        }

        string name = amenityDto.Name.Trim();

        bool alreadyExists =
            await _amenityRepository.ExistsByNameAsync(
                name,
                id
            );

        if (alreadyExists)
        {
            throw new ArgumentException(
                "An amenity with this name already exists."
            );
        }

        amenity.Name = name;
        amenity.Description =
            amenityDto.Description?.Trim();

        await _amenityRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Amenity? amenity =
            await _amenityRepository.GetByIdAsync(id);

        if (amenity == null)
        {
            return false;
        }

        await _amenityRepository.DeleteAsync(
            amenity
        );

        return true;
    }

    private AmenityDto ToDto(Amenity amenity)
    {
        return new AmenityDto
        {
            Id = amenity.Id,
            Name = amenity.Name,
            Description = amenity.Description
        };
    }
}