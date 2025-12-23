using Microsoft.AspNetCore.Mvc;

namespace StaffEventOrganizer.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            return View();
        }
    }
}
