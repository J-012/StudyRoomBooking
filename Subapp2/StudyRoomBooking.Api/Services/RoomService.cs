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

        return rooms.Select(room => room.ToDto()).ToList();
    }

    public async Task<List<RoomDto>> GetAvailableAsync(
        DateTime startTime,
        DateTime endTime,
        int? excludedSessionId)
    {
        if (endTime <= startTime)
        {
            throw new ArgumentException("The end time must be after the start time.");
        }

        // One query for the booked rooms
        List<int> bookedRoomIds =
            await _bookingRepository.GetBookedRoomIdsAsync(startTime, endTime, excludedSessionId);

        List<Room> rooms = await _roomRepository.GetAllAsync();

        return rooms
            .Where(room => !bookedRoomIds.Contains(room.Id))
            .Select(room => room.ToDto())
            .ToList();
    }

    public async Task<RoomDto?> GetByIdAsync(int id)
    {
        Room? room = await _roomRepository.GetByIdAsync(id);

        return room?.ToDto();
    }

    public async Task<RoomDto> CreateAsync(RoomRequestDto roomDto)
    {
        Room room = new Room();

        await ApplyAsync(room, roomDto);

        Room createdRoom = await _roomRepository.AddAsync(room);

        return createdRoom.ToDto();
    }

    public async Task<bool> UpdateAsync(int id, RoomRequestDto roomDto)
    {
        Room? room = await _roomRepository.GetByIdAsync(id);

        if (room == null)
        {
            return false;
        }

        await ApplyAsync(room, roomDto);

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

        // Deleting a room cascades to its bookings.
        if (await _bookingRepository.HasUpcomingBookingsForRoomAsync(id))
        {
            throw new ArgumentException("This room has upcoming bookings. Delete or move the bookings first.");
        }

        await _roomRepository.DeleteAsync(room);

        return true;
    }

    // Shared by create and update: validates the request and copies it onto the room.
    private async Task ApplyAsync(Room room, RoomRequestDto roomDto)
    {
        if (await _buildingRepository.GetByIdAsync(roomDto.BuildingId) == null)
        {
            throw new ArgumentException("Selected building does not exist.");
        }

        List<int> amenityIds = roomDto.AmenityIds.Distinct().ToList();

        if (amenityIds.Count > 0)
        {
            List<Amenity> amenities = await _amenityRepository.GetByIdsAsync(amenityIds);

            if (amenities.Count != amenityIds.Count)
            {
                throw new ArgumentException("One or more selected amenities do not exist.");
            }
        }

        room.RoomNumber = roomDto.RoomNumber;
        room.BuildingId = roomDto.BuildingId;
        room.Capacity = roomDto.Capacity;

        // Replace the whole amenity list with the one from the request.
        room.RoomAmenities.Clear();
        room.RoomAmenities.AddRange(
            amenityIds.Select(amenityId => new RoomAmenity { AmenityId = amenityId }));
    }
}
