using Microsoft.AspNetCore.Mvc;

namespace EventOrganizer.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Order()
        {
            return View();
        }
    }
}
