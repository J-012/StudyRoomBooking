using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly StudyRoomDbContext _context;

    public RoomRepository(StudyRoomDbContext context)
    {
        _context = context;
    }

    // Get all rooms.
    public Task<List<Room>> GetAllAsync()
    {
        return _context.Rooms.ToListAsync();
    }

    // Get a specific room by ID.
    public Task<Room?> GetByIdAsync(int id)
    {
        return _context.Rooms.FirstOrDefaultAsync(room => room.Id == id);
    }

    // Check whether a room exists by ID.
    public Task<bool> ExistsAsync(int id)
    {
        return _context.Rooms.AnyAsync(room => room.Id == id);
    }

    public async Task AddAsync(Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
    }

    // Update an existing room.
    public async Task UpdateAsync(Room room)
    {
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
    }

    // Delete an existing room.
    public async Task DeleteAsync(Room room)
    {
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
    }
}
