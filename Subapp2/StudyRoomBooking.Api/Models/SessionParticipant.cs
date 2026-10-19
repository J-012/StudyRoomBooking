namespace StudyRoomBooking.Api.Models;

public class SessionParticipant
{
    public int StudySessionId { get; set; }
    public StudySession? StudySession { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }
}