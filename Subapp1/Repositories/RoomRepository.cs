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

    // Add a new room.
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

    // Search and filter rooms.
    public async Task<List<Room>> SearchAndFilterAsync(
        string? searchTerm,
        string? building,
        int? minCapacity)
    {
        var query = _context.Rooms.AsQueryable();

        // Search by room number or building.
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();

            query = query.Where(room =>
                room.RoomNumber.Contains(searchTerm) ||
                room.Building.Contains(searchTerm));
        }

        // Filter by building.
        if (!string.IsNullOrWhiteSpace(building))
        {
            query = query.Where(room =>
                room.Building == building);
        }

        // Filter by minimum capacity.
        if (minCapacity.HasValue)
        {
            query = query.Where(room =>
                room.Capacity >= minCapacity.Value);
        }

        return await query.ToListAsync();
    }
}