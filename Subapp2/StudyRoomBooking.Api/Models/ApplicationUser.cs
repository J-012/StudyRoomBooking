using Microsoft.AspNetCore.Identity;

namespace StudyRoomBooking.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;

    public List<SessionParticipant> Participations { get; set; } = new();
}