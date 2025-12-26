using EventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EventOrganizer.Controllers
{

    public class VendorController : Controller
    {

        private readonly IVendor _vendorRepository;
        private readonly ICategory _categoryRepository;

        public VendorController(IVendor vendorRepository, ICategory categoryRepository)
        {
            _vendorRepository = vendorRepository;
            _categoryRepository = categoryRepository;
        }


        public async Task<IActionResult> Index()
        {
            // Ambil UserId dari session
            var userIdString = HttpContext.Session.GetString("UserId");

            var userId = Guid.Parse(userIdString);

            // Cek apakah vendor untuk user ini sudah ada
            var vendor = await _vendorRepository.GetVendorByUserId(userId);

            if (vendor == null)
            {
                TempData["AlertDataMessage"] = "Silakan lengkapi data vendor Anda terlebih dahulu!";
                // buat model kosong agar View aman
                vendor = new VendorModel
                {
                    CompanyName = "Vendor",
                    Status = "Belum Tersedia",
                    VendorId = Guid.Empty
                };
            }
            return View(vendor);
        }

        public async Task<IActionResult> Create()
        {
            var userId = Guid.Parse(HttpContext.Session.GetString("UserId"));

            var vendor = new VendorModel
            {
                UserId = userId,
                Email = HttpContext.Session.GetString("Email") // atau ambil dari repo Users
            };

            ViewBag.Categories = await _categoryRepository.GetAll();
            return View(vendor);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendorModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryRepository.GetAll();
                return View(model);
            }

            await _vendorRepository.Add(model);

            TempData["SuccessMessage"] = "Profil vendor berhasil dibuat";
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit()
        {
            var userId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            var vendor = await _vendorRepository.GetVendorByUserId(userId);

            if (vendor == null)
                return RedirectToAction("Create");

            var categories = await _categoryRepository.GetAll();
            var vendorCategories = await _vendorRepository.GetVendorCategories(vendor.VendorId);

            vendor.SelectedCategoryIds = vendorCategories
                .Select(c => c.CategoryId)
                .ToList();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategories = vendorCategories;
            return View(vendor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VendorModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryRepository.GetAll();
                return View(model);
            }

            // 🔹 update vendor
            await _vendorRepository.UpdateVendorInfo(model);

            // 🔹 update phone (USER)
            await _vendorRepository.UpdateUserPhone(model.UserId, model.PhoneNumber);

            // 🔹 update kategori
            await _vendorRepository.AddVendorCategories(
                model.VendorId,
                model.SelectedCategoryIds
            );

            TempData["SuccessMessage"] = "Profil vendor berhasil diperbarui";
            return RedirectToAction("Index");
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(Guid vendorId)
        {
            // Ambil vendor dulu
            var vendor = await _vendorRepository.GetById(vendorId);
            if (vendor == null)
            {
                TempData["Error"] = "Vendor tidak ditemukan!";
                return RedirectToAction("Index");
            }

            // Toggle status
            var newStatus = vendor.Status == "available" ? "inactive" : "available";
            await _vendorRepository.UpdateStatus(vendorId, newStatus);

            TempData["SuccessMessage"] = $"Status vendor berhasil diubah menjadi {(newStatus == "available" ? "Aktif" : "Tidak Aktif")}.";
            return RedirectToAction("Index");
        }

        public IActionResult Pemesanan()
        {
            return View();
        }

    }
}
