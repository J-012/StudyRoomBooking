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

    public async Task<List<Room>> GetRoomsWithBookingsAsync(
    DateTime startTime,
    DateTime endTime)
    {
        return await _context.Rooms
            .AsNoTracking()
            .Include(r => r.Bookings
                .Where(b => b.StartTime < endTime &&
                            b.EndTime > startTime))
                .ThenInclude(b => b.Participants)
                    .ThenInclude(bp => bp.Participant)
            .ToListAsync();
    }

    public async Task<List<Room>> SearchWithBookingsAsync(
        string? searchTerm,
        string? building,
        int? minCapacity,
        DateTime startTime,
        DateTime endTime)
    {
        var query = _context.Rooms
            .AsNoTracking()
            .AsQueryable();

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
            building = building.Trim();

            query = query.Where(room =>
                room.Building == building);
        }

        // Filter by minimum capacity.
        if (minCapacity.HasValue)
        {
            query = query.Where(room =>
                room.Capacity >= minCapacity.Value);
        }

        // Load bookings that overlap the selected time interval.
        return await query
            .Include(room => room.Bookings
                .Where(booking =>
                    booking.StartTime < endTime &&
                    booking.EndTime > startTime))
                .ThenInclude(booking => booking.Participants)
                    .ThenInclude(bp => bp.Participant)
            .OrderBy(room => room.Building)
            .ThenBy(room => room.RoomNumber)
            .ToListAsync();
    }

}