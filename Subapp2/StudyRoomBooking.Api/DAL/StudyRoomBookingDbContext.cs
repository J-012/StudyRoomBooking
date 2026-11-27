using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.DAL;

public class StudyRoomBookingDbContext : IdentityDbContext<ApplicationUser>
{
    public StudyRoomBookingDbContext(
        DbContextOptions<StudyRoomBookingDbContext> options)
        : base(options)
    {
    }

public DbSet<Room> Rooms { get; set; }
public DbSet<StudySession> StudySessions { get; set; }
public DbSet<SessionParticipant> SessionParticipants { get; set; }
public DbSet<Amenity> Amenities { get; set; }
public DbSet<Building> Buildings { get; set; }
public DbSet<RoomAmenity> RoomAmenities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StudySession>()
            .HasOne(session => session.Organizer)
            .WithMany(user => user.OrganizedSessions)
            .HasForeignKey(session => session.OrganizerUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Room>()
            .HasOne(room => room.Building)
            .WithMany(building => building.Rooms)
            .HasForeignKey(room => room.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudySession>()
            .HasIndex(session => new
            {
                session.RoomId,
                session.StartTime,
                session.EndTime
            });

        modelBuilder.Entity<Room>().HasData(
    new Room
    {
        Id = 1,
        RoomNumber = "PH170",
        BuildingId = 1,
        Capacity = 6
    },
    new Room
    {
        Id = 2,
        RoomNumber = "PH171",
        BuildingId = 1,
        Capacity = 8
    },
    new Room
    {
        Id = 3,
        RoomNumber = "P48-101",
        BuildingId = 2,
        Capacity = 4
    }
);

        modelBuilder.Entity<Amenity>()
    .HasIndex(amenity => amenity.Name)
    .IsUnique();

    modelBuilder.Entity<Building>()
    .HasIndex(building => building.Name)
    .IsUnique();

modelBuilder.Entity<Building>().HasData(
    new Building
    {
        Id = 1,
        Name = "Pilestredet 35"
    },
    new Building
    {
        Id = 2,
        Name = "Pilestredet 48"
    }
);

modelBuilder.Entity<RoomAmenity>()
    .HasKey(roomAmenity => new
    {
        roomAmenity.RoomId,
        roomAmenity.AmenityId
    });

modelBuilder.Entity<RoomAmenity>()
    .HasOne(roomAmenity => roomAmenity.Room)
    .WithMany(room => room.RoomAmenities)
    .HasForeignKey(roomAmenity => roomAmenity.RoomId);

modelBuilder.Entity<RoomAmenity>()
    .HasOne(roomAmenity => roomAmenity.Amenity)
    .WithMany(amenity => amenity.RoomAmenities)
    .HasForeignKey(roomAmenity => roomAmenity.AmenityId);

modelBuilder.Entity<Amenity>().HasData(
    new Amenity
    {
        Id = 1,
        Name = "Whiteboard",
        Description = "Whiteboard available in the room."
    },
    new Amenity
    {
        Id = 2,
        Name = "Monitor",
        Description = "External monitor available."
    },
    new Amenity
    {
        Id = 3,
        Name = "Power outlets",
        Description = "Power outlets available for students."
    }
);

        modelBuilder.Entity<RoomAmenity>().HasData(
            new RoomAmenity
            {
                RoomId = 1,
                AmenityId = 1
            },
            new RoomAmenity
            {
                RoomId = 1,
                AmenityId = 2
            },
            new RoomAmenity
            {
                RoomId = 2,
                AmenityId = 1
            },
            new RoomAmenity
            {
                RoomId = 2,
                AmenityId = 3
            },
            new RoomAmenity
            {
                RoomId = 3,
                AmenityId = 3
            }
        );

        modelBuilder.Entity<SessionParticipant>()
            .HasKey(sp => new { sp.StudySessionId, sp.UserId });

        modelBuilder.Entity<SessionParticipant>()
            .HasOne(sp => sp.StudySession)
            .WithMany(s => s.Participants)
            .HasForeignKey(sp => sp.StudySessionId);

        modelBuilder.Entity<SessionParticipant>()
            .HasOne(sp => sp.User)
            .WithMany(u => u.Participations)
            .HasForeignKey(sp => sp.UserId);
    }
}