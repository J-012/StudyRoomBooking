using StudyRoomBooking.ViewModels.Admin;

namespace StudyRoomBooking.Repositories;

public interface IAdminOverviewRepository
{
    Task<AdminOverviewViewModel> GetOverviewAsync();
}
