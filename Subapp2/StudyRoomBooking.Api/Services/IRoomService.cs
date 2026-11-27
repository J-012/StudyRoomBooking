using StudyRoomBooking.Api.DTOs;

namespace StudyRoomBooking.Api.Services;

// Update/Delete return false when the room does not exist
// Broken rules throw ArgumentException
public interface IRoomService
{
    Task<List<RoomDto>> GetAllAsync();

    // Rooms with no booking overlapping the time range
    Task<List<RoomDto>> GetAvailableAsync(DateTime startTime, DateTime endTime, int? excludedSessionId);

    Task<RoomDto?> GetByIdAsync(int id);

    Task<RoomDto> CreateAsync(RoomRequestDto roomDto);

    Task<bool> UpdateAsync(int id, RoomRequestDto roomDto);

    Task<bool> DeleteAsync(int id);
}
