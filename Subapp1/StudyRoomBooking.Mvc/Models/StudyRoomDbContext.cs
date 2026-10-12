using Microsoft.EntityFrameworkCore;

namespace StudyRoomBooking.Models
{
    public class StudyRoomDbContext : DbContext
    {
        public StudyRoomDbContext(DbContextOptions<StudyRoomDbContext> options): base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        
    } 
}
