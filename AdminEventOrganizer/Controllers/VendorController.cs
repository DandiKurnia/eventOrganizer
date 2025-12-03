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
                    (v.Category ?? "").ToLower().Contains(search) ||
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
            return View(vendor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, VendorModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = await _vendorRepository.GetById(id);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "Vendor tidak ditemukan.";
                return RedirectToAction(nameof(Index));
            }

            existing.CompanyName = model.CompanyName;
            existing.Category = model.Category;
            existing.Email = model.Email;
            existing.Phone = model.Phone;
            existing.Status = model.Status;
            existing.Address = model.Address;

            await _vendorRepository.Update(existing);

            TempData["SuccessMessage"] = "Data vendor berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _vendorRepository.Delete(id);
            TempData["SuccessMessage"] = "Vendor berhasil dihapus.";
            return RedirectToAction(nameof(Index));
        }
    }
}
