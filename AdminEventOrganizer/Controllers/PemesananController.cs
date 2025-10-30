using AdminEventOrganizer.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AdminEventOrganizer.Controllers
{
    [TypeFilter(typeof(AuthorizationFilter))]

    public class PemesananController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
