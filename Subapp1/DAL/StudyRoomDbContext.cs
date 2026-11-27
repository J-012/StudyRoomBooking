using Microsoft.EntityFrameworkCore;

namespace StudyRoomBooking.Models
{
    public class StudyRoomDbContext : DbContext
    {
        public StudyRoomDbContext(DbContextOptions<StudyRoomDbContext> options)
            : base(options) { }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Participant> Participants { get; set; } = null!;

        public DbSet<BookingParticipant> BookingParticipants { get; set; } = null!;
        public DbSet<Building> Buildings => Set<Building>();
        public DbSet<Amenity> Amenities => Set<Amenity>();
        public DbSet<RoomAmenity> RoomAmenities => Set<RoomAmenity>();
        public DbSet<Course> Courses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<User>()
                .Property(user => user.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder
                .Entity<Room>()
                .HasData(
                    new Room
                    {
                        Id = 1,
                        RoomNumber = "101",
                        BuildingId = 1,
                        Capacity = 6,
                    },
                    new Room
                    {
                        Id = 2,
                        RoomNumber = "102",
                        BuildingId = 1,
                        Capacity = 6,
                    },
                    new Room
                    {
                        Id = 3,
                        RoomNumber = "103",
                        BuildingId = 1,
                        Capacity = 12,
                    },
                    new Room
                    {
                        Id = 4,
                        RoomNumber = "201",
                        BuildingId = 1,
                        Capacity = 6,
                    },
                    new Room
                    {
                        Id = 5,
                        RoomNumber = "202",
                        BuildingId = 1,
                        Capacity = 6,
                    },
                    new Room
                    {
                        Id = 6,
                        RoomNumber = "101",
                        BuildingId = 2,
                        Capacity = 8,
                    },
                    new Room
                    {
                        Id = 7,
                        RoomNumber = "102",
                        BuildingId = 2,
                        Capacity = 8,
                    }
                );

            modelBuilder.Entity<Building>(entity =>
            {
                entity.HasIndex(b => b.Name).IsUnique();
                entity.HasMany(b => b.Rooms).WithOne(r => r.Building)
                    .HasForeignKey(r => r.BuildingId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Amenity>().HasIndex(a => a.Name).IsUnique();
            modelBuilder.Entity<RoomAmenity>(entity =>
            {
                entity.HasKey(ra => new { ra.RoomId, ra.AmenityId });
                entity.HasOne(ra => ra.Room).WithMany(r => r.RoomAmenities)
                    .HasForeignKey(ra => ra.RoomId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ra => ra.Amenity).WithMany(a => a.RoomAmenities)
                    .HasForeignKey(ra => ra.AmenityId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder
                .Entity<Booking>()
                .HasOne(booking => booking.User)
                .WithMany(user => user.Bookings)
                .HasForeignKey(booking => booking.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Booking>()
                .HasOne(booking => booking.Room)
                .WithMany(room => room.Bookings)
                .HasForeignKey(booking => booking.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>().HasKey(c => c.CourseCode);

            modelBuilder
                .Entity<Booking>()
                .HasOne(b => b.Course)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CourseCode)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.HasKey(p => p.ParticipantId);

                entity.HasIndex(p => p.UserId).IsUnique();

                entity
                    .HasOne(p => p.User)
                    .WithOne(u => u.Participant)
                    .HasForeignKey<Participant>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BookingParticipant>(entity =>
            {
                entity.HasKey(bp => new { bp.BookingId, bp.ParticipantId });

                entity
                    .HasOne(bp => bp.Booking)
                    .WithMany(b => b.Participants)
                    .HasForeignKey(bp => bp.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(bp => bp.Participant)
                    .WithMany(p => p.BookingParticipants)
                    .HasForeignKey(bp => bp.ParticipantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
