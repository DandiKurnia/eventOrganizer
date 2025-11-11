using AdminEventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AdmEventOrganizer.Controllers
{
    public class VendorController : Controller
    {
        private readonly IVendor vendorRepository;

        public VendorController(IVendor vendorRepository)
        {
            this.vendorRepository = vendorRepository;
        }
        public async Task <IActionResult> Index()
        {
            var vendors = await vendorRepository.Get();
            return View(vendors);
        }
    }
}
