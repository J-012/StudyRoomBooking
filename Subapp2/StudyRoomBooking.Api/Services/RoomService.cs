using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Repositories;

namespace StudyRoomBooking.Api.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;

    public RoomService(
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
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
        Room room = new Room
        {
            RoomNumber = roomDto.RoomNumber,
            Building = roomDto.Building,
            Capacity = roomDto.Capacity
        };

        Room createdRoom = await _roomRepository.AddAsync(room);

        return ToDto(createdRoom);
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

        room.RoomNumber = roomDto.RoomNumber;
        room.Building = roomDto.Building;
        room.Capacity = roomDto.Capacity;

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

        await _roomRepository.DeleteAsync(room);

        return true;
    }

    private RoomDto ToDto(Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            Building = room.Building,
            Capacity = room.Capacity
        };
    }
}