using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public interface IUserRepository
{
    // Hent en bruker basert på bruker ID
    Task<User?> GetByUserIdAsync(string userId);

    // Sjekk om en bruker eksisterer basert på bruker ID
    Task<bool> ExistsAsync(string userId);

    // Legg til en ny bruker
    Task<User> AddAsync(User user);
}
