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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<User>()
                .Property(user => user.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

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

        public DbSet<Course> Courses { get; set; } = null!;
    }
}
