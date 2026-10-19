using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.DAL;

public class StudyRoomBookingDbContext : DbContext
{
    public StudyRoomBookingDbContext(
        DbContextOptions<StudyRoomBookingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudySession> StudySessions { get; set; }
    public DbSet<SessionParticipant> SessionParticipants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Room>().HasData(
            new Room
            {
                Id = 1,
                RoomNumber = "PH170",
                Building = "Pilestredet 35",
                Capacity = 6
            },
            new Room
            {
                Id = 2,
                RoomNumber = "PH171",
                Building = "Pilestredet 35",
                Capacity = 8
            },
            new Room
            {
                Id = 3,
                RoomNumber = "P48-101",
                Building = "Pilestredet 48",
                Capacity = 4
            }
        );

        // Sammensatt primærnøkkel for kobletabellen (ren N:M)
        modelBuilder.Entity<SessionParticipant>()
            .HasKey(sp => new { sp.StudySessionId, sp.StudentId });

        modelBuilder.Entity<SessionParticipant>()
            .HasOne(sp => sp.StudySession)
            .WithMany(s => s.Participants)
            .HasForeignKey(sp => sp.StudySessionId);

        modelBuilder.Entity<SessionParticipant>()
            .HasOne(sp => sp.Student)
            .WithMany(s => s.Participations)
            .HasForeignKey(sp => sp.StudentId);
    }
}