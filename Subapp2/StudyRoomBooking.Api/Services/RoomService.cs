using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Repositories;

namespace StudyRoomBooking.Api.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IAmenityRepository _amenityRepository;
    private readonly IBuildingRepository _buildingRepository;

    public RoomService(
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository,
        IAmenityRepository amenityRepository,
        IBuildingRepository buildingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
        _amenityRepository = amenityRepository;
        _buildingRepository = buildingRepository;
    }

    public async Task<List<RoomDto>> GetAllAsync()
    {
        List<Room> rooms = await _roomRepository.GetAllAsync();

        List<RoomDto> roomDtos = new List<RoomDto>();

        foreach (Room room in rooms)
        {
            roomDtos.Add(ToDto(room));
        }

        return roomDtos;
    }

    // Returnerer rom som ikke har noen booking som overlapper med
    // tidsrommet. excludedSessionId brukes når en booking endres,
    // så den ikke kolliderer med seg selv.
    public async Task<List<RoomDto>> GetAvailableAsync(
        DateTime startTime,
        DateTime endTime,
        int? excludedSessionId)
    {
        if (endTime <= startTime)
        {
            throw new ArgumentException(
                "The end time must be after the start time."
            );
        }

        List<Room> rooms = await _roomRepository.GetAllAsync();

        List<RoomDto> availableRooms = new List<RoomDto>();

        foreach (Room room in rooms)
        {
            bool hasConflict =
                await _bookingRepository.HasConflictAsync(
                    room.Id,
                    startTime,
                    endTime,
                    excludedSessionId
                );

            if (!hasConflict)
            {
                availableRooms.Add(ToDto(room));
            }
        }

        return availableRooms
            .OrderBy(room => room.Building)
            .ThenBy(room => room.RoomNumber)
            .ToList();
    }

    public async Task<RoomDto?> GetByIdAsync(int id)
    {
        Room? room = await _roomRepository.GetByIdAsync(id);

        if (room == null)
        {
            return null;
        }

        return ToDto(room);
    }

    public async Task<RoomDto> CreateAsync(CreateRoomDto roomDto)
    {
        Building? building =
            await _buildingRepository.GetByIdAsync(roomDto.BuildingId);

        if (building == null)
        {
            throw new ArgumentException(
                "Selected building does not exist."
            );
        }

        List<int> amenityIds =
            await ValidateAmenityIdsAsync(roomDto.AmenityIds);

        Room room = new Room
        {
            RoomNumber = roomDto.RoomNumber,
            BuildingId = roomDto.BuildingId,
            Capacity = roomDto.Capacity
        };

        foreach (int amenityId in amenityIds)
        {
            room.RoomAmenities.Add(
                new RoomAmenity
                {
                    AmenityId = amenityId
                }
            );
        }

        Room createdRoom = await _roomRepository.AddAsync(room);

        Room? completeRoom =
            await _roomRepository.GetByIdAsync(createdRoom.Id);

        return ToDto(completeRoom!);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateRoomDto roomDto)
    {
        Room? room = await _roomRepository.GetByIdAsync(id);

        if (room == null)
        {
            return false;
        }

        Building? building =
            await _buildingRepository.GetByIdAsync(roomDto.BuildingId);

        if (building == null)
        {
            throw new ArgumentException(
                "Selected building does not exist."
            );
        }

        List<int> amenityIds =
            await ValidateAmenityIdsAsync(roomDto.AmenityIds);

        room.RoomNumber = roomDto.RoomNumber;
        room.BuildingId = roomDto.BuildingId;
        room.Capacity = roomDto.Capacity;

        room.RoomAmenities.Clear();

        foreach (int amenityId in amenityIds)
        {
            room.RoomAmenities.Add(
                new RoomAmenity
                {
                    RoomId = room.Id,
                    AmenityId = amenityId
                }
            );
        }

        await _roomRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Room? room = await _roomRepository.GetByIdAsync(id);

        if (room == null)
        {
            return false;
        }

        // Deleting a room cascades to its bookings. Refuse while students
        // still have upcoming bookings there, so they do not lose them
        // without warning. Bookings that have already ended are removed
        // together with the room.
        bool hasUpcomingBookings =
            await _bookingRepository
                .HasUpcomingBookingsForRoomAsync(id);

        if (hasUpcomingBookings)
        {
            throw new ArgumentException(
                "This room has upcoming bookings. Delete or move the bookings first."
            );
        }

        await _roomRepository.DeleteAsync(room);

        return true;
    }

    private RoomDto ToDto(Room room)
    {
        RoomDto roomDto = new RoomDto
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            BuildingId = room.BuildingId,
            Building = room.Building?.Name ?? string.Empty,
            Capacity = room.Capacity
        };

        foreach (RoomAmenity roomAmenity in room.RoomAmenities)
        {
            if (roomAmenity.Amenity != null)
            {
                roomDto.Amenities.Add(
                    new AmenityDto
                    {
                        Id = roomAmenity.Amenity.Id,
                        Name = roomAmenity.Amenity.Name,
                        Description = roomAmenity.Amenity.Description
                    }
                );
            }
        }

        return roomDto;
    }

    private async Task<List<int>> ValidateAmenityIdsAsync(
        List<int> amenityIds)
    {
        List<int> uniqueIds = amenityIds.Distinct().ToList();

        if (uniqueIds.Count == 0)
        {
            return uniqueIds;
        }

        List<Amenity> amenities =
            await _amenityRepository.GetByIdsAsync(uniqueIds);

        if (amenities.Count != uniqueIds.Count)
        {
            throw new ArgumentException(
                "One or more selected amenities do not exist."
            );
        }

        return uniqueIds;
    }
}
