using EventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EventOrganizer.Controllers
{
    public class LandingPageController : Controller
    {
        private readonly IPackageEvent _packageEventRepo;

        public LandingPageController(IPackageEvent packageEventRepo)
        {
            _packageEventRepo = packageEventRepo;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new LandingPageViewModel
            {
                ActivePackages = (await _packageEventRepo.GetActivePackages()).ToList()
            };

            return View(viewModel);
        }
    }
}
