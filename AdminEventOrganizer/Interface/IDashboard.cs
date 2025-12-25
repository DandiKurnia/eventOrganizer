using Models;

namespace AdminEventOrganizer.Interface
{
    public interface IDashboard
    {
        Task<DashboardViewModel> GetDashboardSummary();
    }

}
