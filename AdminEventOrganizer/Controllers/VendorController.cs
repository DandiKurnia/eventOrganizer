using AdminEventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AdminEventOrganizer.Controllers
{
    public class VendorController : Controller
    {
        private readonly IVendor _vendorRepository;

        public VendorController(IVendor vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            const int pageSize = 10;
            var vendors = await _vendorRepository.Get();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                vendors = vendors.Where(v =>
                    v.CompanyName.ToLower().Contains(search) ||
                    (v.Status ?? "").ToLower().Contains(search));
            }

            var totalItems = vendors.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var data = vendors
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;

            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var vendor = await _vendorRepository.GetById(id);
            if (vendor == null)
            {
                TempData["ErrorMessage"] = "Vendor tidak ditemukan.";
                return RedirectToAction(nameof(Index));
            }

            // 🔥 AMBIL KATEGORI VENDOR
            var categories = await _vendorRepository.GetVendorCategories(id);
            ViewBag.VendorCategories = categories;

            return View(vendor);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var vendor = await _vendorRepository.GetById(id);
            if (vendor == null)
            {
                TempData["ErrorMessage"] = "Vendor tidak ditemukan.";
                return RedirectToAction(nameof(Index));
            }

            return View(new EditVendorStatusViewModel
            {
                VendorId = vendor.VendorId,
                Status = vendor.Status
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditVendorStatusViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _vendorRepository.UpdateStatus(model.VendorId, model.Status);

            TempData["SuccessMessage"] = "Status vendor berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

    }
}
