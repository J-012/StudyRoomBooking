using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public interface IRoomRepository
{
    // Get all rooms
    Task<List<Room>> GetAllAsync();

    // Hent et spesifikt rom basert på ID
    Task<Room?> GetByIdAsync(int id);

    // Sjekk om et rom eksisterer basert på ID
    Task<bool> ExistsAsync(int id);

    // Legg til et nytt rom
    Task AddAsync(Room room);

    // Oppdater et eksisterende rom
    Task UpdateAsync(Room room);

    // Slett et eksisterende rom
    Task DeleteAsync(Room room);
}
