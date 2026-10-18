using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Repositories;

namespace StudyRoomBooking.Api.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
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