using Microsoft.AspNetCore.Identity;

namespace StudyRoomBooking.Api.Models;

// Identity user extended with student info. Used for both students and admins.
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;

    // Sessions the user has joined, and sessions the user has created.
    public List<SessionParticipant> Participations { get; set; } = new();
    public List<StudySession> OrganizedSessions { get; set; } = new();
}
