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

    // Hent alle rom
    public Task<List<Room>> GetAllAsync()
    {
        return _context.Rooms.ToListAsync();
    }

    // Hent et spesifikt rom basert på ID
    public Task<Room?> GetByIdAsync(int id)
    {
        return _context.Rooms.FirstOrDefaultAsync(room => room.Id == id);
    }

    // Sjekk om et rom eksisterer basert på ID
    public Task<bool> ExistsAsync(int id)
    {
        return _context.Rooms.AnyAsync(room => room.Id == id);
    }
}
