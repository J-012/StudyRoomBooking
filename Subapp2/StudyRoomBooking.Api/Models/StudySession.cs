using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyRoomBooking.Api.Models;

public class StudySession
{
    public int Id { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [StringLength(200)]
    public string? Topic { get; set; }

    // 1:mange - en sesjon hører til ett rom
    public int RoomId { get; set; }

    [ForeignKey(nameof(RoomId))]
    public Room? Room { get; set; }

    // N:M - en sesjon har flere studenter, via kobletabellen
    public List<SessionParticipant> Participants { get; set; } = new();
}