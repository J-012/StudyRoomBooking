using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Models;
using StudyRoomBooking.ViewModels.Admin;

namespace StudyRoomBooking.Repositories;

public class AdminOverviewRepository(StudyRoomDbContext context) : IAdminOverviewRepository
{
    public async Task<AdminOverviewViewModel> GetOverviewAsync() => new()
    {
        RoomCount = await context.Rooms.CountAsync(),
        BuildingCount = await context.Buildings.CountAsync(),
        AmenityCount = await context.Amenities.CountAsync(),
        CourseCount = await context.Courses.CountAsync(),
        BookingCount = await context.Bookings.CountAsync(),
        UserCount = await context.Users.CountAsync()
    };
}
