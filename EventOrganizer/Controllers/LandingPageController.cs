using Microsoft.AspNetCore.Mvc;

namespace EventOrganizer.Controllers
{
    public class LandingPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
