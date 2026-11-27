namespace StudyRoomBooking.Api.Models;

// Join table between StudySession and the users who have joined it.
public class SessionParticipant
{
    public int StudySessionId { get; set; }
    public StudySession? StudySession { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
}
