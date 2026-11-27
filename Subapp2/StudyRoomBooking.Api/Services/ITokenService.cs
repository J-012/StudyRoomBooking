using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Services;

public interface ITokenService
{
    Task<string> CreateTokenAsync(ApplicationUser user);
}