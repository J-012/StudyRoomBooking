using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Api.DAL;
using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly StudyRoomBookingDbContext _context;

    public RoomRepository(StudyRoomBookingDbContext context)
    {
        _context = context;
    }

    // Rooms with everything RoomDto needs. Shared by all read methods.
    private IQueryable<Room> RoomsWithDetails =>
        _context.Rooms
            .Include(room => room.Building)
            .Include(room => room.RoomAmenities)
                .ThenInclude(roomAmenity => roomAmenity.Amenity);

    public Task<List<Room>> GetAllAsync() =>
        RoomsWithDetails
            .AsNoTracking()
            .OrderBy(room => room.Building!.Name)
            .ThenBy(room => room.RoomNumber)
            .ToListAsync();

    public Task<Room?> GetByIdAsync(int id) =>
        RoomsWithDetails.FirstOrDefaultAsync(room => room.Id == id);

    public async Task<Room> AddAsync(Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // Reload so Building and Amenities are filled in for the response.
        return (await GetByIdAsync(room.Id))!;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public async Task DeleteAsync(Room room)
    {
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
    }
}
