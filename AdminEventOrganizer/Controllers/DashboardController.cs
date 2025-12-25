using AdminEventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AdminEventOrganizer.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboard _dashboardRepo;

        public DashboardController(IDashboard dashboardRepo)
        {
            _dashboardRepo = dashboardRepo;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _dashboardRepo.GetDashboardSummary();
            return View(data);
        }
    }
}
