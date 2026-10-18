using StudyRoomBooking.Api.DTOs;

namespace StudyRoomBooking.Api.Services;

public interface IRoomService
{
    Task<List<RoomDto>> GetAllAsync();

    Task<RoomDto?> GetByIdAsync(int id);

    Task<RoomDto> CreateAsync(CreateRoomDto roomDto);

    Task<bool> UpdateAsync(int id, UpdateRoomDto roomDto);

    Task<bool> DeleteAsync(int id);
}