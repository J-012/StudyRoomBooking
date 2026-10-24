using StudyRoomBooking.Models;

namespace StudyRoomBooking.Services;

public interface IUserService
{
    // Autentiser en bruker basert på bruker ID og passord
    Task<User?> AuthenticateAsync(string? userId, string? password);

    // Registrer en ny bruker
    Task<UserRegistrationResult> RegisterAsync(
        string? userId,
        string? name,
        string? password,
        string? confirmPassword);
}
