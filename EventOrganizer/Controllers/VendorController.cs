using EventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EventOrganizer.Controllers
{

    public class VendorController : Controller
    {

        private readonly IVendor _vendorRepository;
        private readonly ICategory _categoryRepository;
        private readonly IVendorConfirmation _vendorConfirmationRepo;

        public VendorController(
            IVendor vendorRepository,
            ICategory categoryRepository,
            IVendorConfirmation vendorConfirmationRepo)
        {
            _vendorRepository = vendorRepository;
            _categoryRepository = categoryRepository;
            _vendorConfirmationRepo = vendorConfirmationRepo;
        }


        public async Task<IActionResult> Index()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
                return RedirectToAction("Login", "Auth");

            var userId = Guid.Parse(userIdString);

            var vendor = await _vendorRepository.GetVendorByUserId(userId);
            if (vendor == null)
            {
                TempData["AlertDataMessage"] = "Silakan lengkapi data vendor Anda terlebih dahulu!";
                return RedirectToAction("Create");
            }

            // 🔥 DASHBOARD DATA
            var dashboard = await _vendorRepository.GetVendorDashboard(vendor.VendorId);

            return View(dashboard); // Views/Vendor/Index.cshtml
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

        public async Task<IActionResult> Pemesanan(string? search, int page = 1)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
                return RedirectToAction("Login", "Auth");

            var userId = Guid.Parse(userIdString);

            var vendor = await _vendorRepository.GetVendorByUserId(userId);
            if (vendor == null)
            {
                TempData["Error"] = "Vendor tidak ditemukan.";
                return RedirectToAction("Index");
            }

            var orders = await _vendorConfirmationRepo.GetOrdersForVendor(vendor.VendorId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                orders = orders.Where(o =>
                    o.ClientName.ToLower().Contains(search) ||
                    o.PackageName.ToLower().Contains(search) ||
                    o.EventDate.ToString("dd/MM/yyyy").Contains(search)
                );
            }
            const int pageSize = 10;
            var totalItems = orders.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var data = orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(data);
        }

        public async Task<IActionResult> DetailPemesanan(Guid id)
        {
            var userId = Guid.Parse(HttpContext.Session.GetString("UserId"));
            var vendor = await _vendorRepository.GetVendorByUserId(userId);
            if (vendor == null)
                return RedirectToAction("Index");

            var data = await _vendorConfirmationRepo.GetById(id);

            if (data == null || data.VendorId != vendor.VendorId)
            {
                TempData["Error"] = "Data pemesanan tidak ditemukan.";
                return RedirectToAction("Pemesanan");
            }

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(
                Guid vendorConfirmationId,
                decimal actualPrice,
                string? notes)
        {
            var data = await _vendorConfirmationRepo.GetById(vendorConfirmationId);
            if (data == null)
                return RedirectToAction("Pemesanan");

            // update vendor ini
            await _vendorConfirmationRepo.UpdateAccept(
                vendorConfirmationId,
                actualPrice,
                notes
            );

            // 🔥 AUTO CLOSE vendor lain
            await _vendorConfirmationRepo.CloseOtherVendors(
                data.OrderId,
                data.VendorId
            );

            TempData["SuccessMessage"] = "Pemesanan berhasil diterima.";
            return RedirectToAction("Pemesanan");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid vendorConfirmationId)
        {
            await _vendorConfirmationRepo.UpdateStatus(
                vendorConfirmationId,
                "closed"
            );

            TempData["SuccessMessage"] = "Pemesanan ditolak.";
            return RedirectToAction("Pemesanan");
        }






    }
}
