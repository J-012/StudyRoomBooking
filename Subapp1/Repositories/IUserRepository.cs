using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public interface IUserRepository
{
    // Get a user by user ID.
    Task<User?> GetByUserIdAsync(string userId);

    // Check whether a user exists by user ID.
    Task<bool> ExistsAsync(string userId);

    // Add a new user.
    Task<User> AddAsync(User user);
}
