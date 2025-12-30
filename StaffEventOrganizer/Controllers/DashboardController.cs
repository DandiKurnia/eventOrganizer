using Microsoft.AspNetCore.Mvc;
using StaffEventOrganizer.Interface;

namespace StaffEventOrganizer.Controllers
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
            
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            
            return View(data);
        }
    }
}
