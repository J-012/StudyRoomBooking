using StudyRoomBooking.Models;

namespace StudyRoomBooking.ViewModels;

public class BookingsIndexViewModel
{
    public List<Booking> MyBookings { get; set; } = new();
    public List<Booking> JoinedBookings { get; set; } = new();
    public List<Booking> JoinableBookings { get; set; } = new();
}
