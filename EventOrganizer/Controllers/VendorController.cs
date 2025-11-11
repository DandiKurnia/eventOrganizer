using EventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EventOrganizer.Controllers
{

    public class VendorController : Controller
    {

        private readonly IVendor _vendorRepository;

        public VendorController(IVendor vendorRepository)
        {
            _vendorRepository = vendorRepository;
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendorModel model)
        {
            // Ambil UserId dari session
            var userIdString = HttpContext.Session.GetString("UserId");
            var userId = Guid.Parse(userIdString);

            if (ModelState.IsValid)
            {
                // Set UserId agar vendor terhubung ke user
                model.UserId = userId;

                // Simpan ke database lewat repository
                await _vendorRepository.Add(model); // Pastikan Add(VendorModel) sudah ada di repository

                TempData["SuccessMessage"] = "Data vendor berhasil disimpan!";
                return RedirectToAction("Index");
            }

            // Kalau validasi gagal, tetap di halaman Create
            return View(model);
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
