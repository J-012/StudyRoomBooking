using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Repositories;

public class UserRepository : IUserRepository
{
    private readonly StudyRoomDbContext _context;

    public UserRepository(StudyRoomDbContext context)
    {
        _context = context;
    }
    // Get a user by user ID.
    public Task<User?> GetByUserIdAsync(string userId)
    {
        return _context.Users.FirstOrDefaultAsync(user => user.UserId == userId);
    }

    // Check whether a user exists by user ID.
    public Task<bool> ExistsAsync(string userId)
    {
        return _context.Users.AnyAsync(user => user.UserId == userId);
    }

    // Add a new user.
    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
}
