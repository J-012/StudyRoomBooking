using StudyRoomBooking.Models;

namespace StudyRoomBooking.ViewModels;

public class RoomsIndexViewModel
{
    public RoomsIndexViewModel()
    {
        var now = DateTime.Now;
        var currentMinute = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        var minutesToNextQuarter = (15 - currentMinute.Minute % 15) % 15;

        if (now.Second > 0 && minutesToNextQuarter == 0)
        {
            minutesToNextQuarter = 15;
        }

        var start = currentMinute.AddMinutes(minutesToNextQuarter);
        var end = start.AddHours(1);

        if (end.Date != start.Date)
        {
            start = start.Date.AddDays(1);
            end = start.AddHours(1);
        }

        Date = start.Date;
        StartTime = start.TimeOfDay;
        EndTime = end.TimeOfDay;
    }

    public string? SearchTerm { get; set; }
    public string? Building { get; set; }
    public int? MinCapacity { get; set; }

    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    // all, available, booked
    public string Status { get; set; } = "all";

    public List<Room> Rooms { get; set; } = new();
    public List<string> Buildings { get; set; } = new();
}
