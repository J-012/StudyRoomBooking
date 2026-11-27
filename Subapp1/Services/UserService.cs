using Microsoft.AspNetCore.Identity;
using StudyRoomBooking.Models;
using StudyRoomBooking.Repositories;

namespace StudyRoomBooking.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> AuthenticateAsync(string? userId, string? password)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var user = await _userRepository.GetByUserIdAsync(userId.Trim());

        return user is not null &&
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password)
                != PasswordVerificationResult.Failed
            ? user
            : null;
    }

    public async Task<UserRegistrationResult> RegisterAsync(
        string? userId,
        string? name,
        string? password,
        string? email,
        string? confirmPassword)
    {
        userId = userId?.Trim() ?? string.Empty;
        name = name?.Trim() ?? string.Empty;
        email = email?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return UserRegistrationResult.Failure("User ID is required.");
        }

        if (userId.Length < 3 || userId.Length > 50)
        {
            return UserRegistrationResult.Failure("User ID must be between 3 and 50 characters.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return UserRegistrationResult.Failure("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return UserRegistrationResult.Failure("Email is required.");
        }

        if (!email.Contains("@"))
        {
            return UserRegistrationResult.Failure("Email is not valid.");
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            return UserRegistrationResult.Failure("Password must be at least 8 characters.");
        }

        if (password != confirmPassword)
        {
            return UserRegistrationResult.Failure("Passwords do not match.");
        }

        if (await _userRepository.ExistsAsync(userId))
        {
            return UserRegistrationResult.Failure("This user ID is already registered.");
        }

        var user = new User { UserId = userId, Name = name, Role = UserRole.Student, Email = email };
        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        await _userRepository.AddAsync(user);

        return UserRegistrationResult.Success();
    }
}
