using Models;

namespace StaffEventOrganizer.Interface
{
    public interface IDashboard
    {
        Task<DashboardViewModel> GetDashboardSummary();
    }

}
