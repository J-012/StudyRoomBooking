using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Services;

public interface ITokenService
{
    // Creates a signed JWT with the user's id, email, name and roles.
    string CreateToken(ApplicationUser user, IEnumerable<string> roles);
}
