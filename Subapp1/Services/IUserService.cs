using StudyRoomBooking.Models;

namespace StudyRoomBooking.Services;

public interface IUserService
{
    // Authenticate a user by user ID and password.
    Task<User?> AuthenticateAsync(string? userId, string? password);

    // Register a new user.
    Task<UserRegistrationResult> RegisterAsync(
        string? userId,
        string? name,
        string? password,
        string? email,
        string? confirmPassword);
}
