using Microsoft.EntityFrameworkCore;
namespace StudyRoomBooking.Models
{
    public class StudyRoomDbContext : DbContext
    {
        public StudyRoomDbContext(DbContextOptions<StudyRoomDbContext> options): base(options){}

        public DbSet<Room> Rooms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(user => user.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, RoomNumber = "101", Building = "Pilestredet 35", Capacity = 6 },
                new Room { Id = 2, RoomNumber = "102", Building = "Pilestredet 35", Capacity = 6 },
                new Room { Id = 3, RoomNumber = "103", Building = "Pilestredet 35", Capacity = 12 },
                new Room { Id = 4, RoomNumber = "201", Building = "Pilestredet 35", Capacity = 6 },
                new Room { Id = 5, RoomNumber = "202", Building = "Pilestredet 35", Capacity = 6 },
                new Room { Id = 6, RoomNumber = "101", Building = "Pilestredet 32", Capacity = 8 },
                new Room { Id = 7, RoomNumber = "102", Building = "Pilestredet 32", Capacity = 8 });

            modelBuilder.Entity<Booking>()
                .HasOne(booking => booking.User)
                .WithMany(user => user.Bookings)
                .HasForeignKey(booking => booking.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(booking => booking.Room)
                .WithMany(room => room.Bookings)
                .HasForeignKey(booking => booking.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
