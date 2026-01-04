using EventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Linq;

namespace EventOrganizer.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IPackageEvent _packageEventRepository;
        private readonly IPackagePhoto _packagePhotoRepository;
        private readonly IOrder _orderRepo;
        private readonly ICategory _categoryRepo;


        public CustomerController(
            IPackageEvent packageEventRepository,
            IOrder orderRepo,
            IPackagePhoto packagePhotoRepository,
            ICategory categoryRepo)
        {
            _packageEventRepository = packageEventRepository;
            _orderRepo = orderRepo;
            _packagePhotoRepository = packagePhotoRepository;
            _categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index(string? search, Guid? category)
        {
            var packages = await _packageEventRepository.Get(search, category);
            var categories = await _categoryRepo.GetAll();

            foreach (var package in packages)
            {
                var photos = await _packagePhotoRepository.GetByPackageId(package.PackageEventId);
                var first = photos.FirstOrDefault();

                package.ThumbnailUrl = first != null
                    ? Url.Action("getFile", "File", new { id = first.PhotoId })
                    : null;
            }

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = category;

            return View(packages);
        }




        public async Task<IActionResult> Detail(Guid id)
        {
            var package = await _packageEventRepository.GetById(id);
            if (package == null)
                return NotFound();
            var photos = await _packagePhotoRepository.GetByPackageId(id);


            package.Photos = photos?.ToList() ?? new List<PackagePhoto>();

            return View(package);
        }


        [HttpPost]
        public async Task<IActionResult> OrderPackage(Guid packageEventId, DateTime eventDate, string additionalRequest)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "Silakan login terlebih dahulu!";
                return RedirectToAction("Login", "User");
            }

            var order = new OrderModel
            {
                OrderId = Guid.NewGuid(),
                UserId = Guid.Parse(userId),
                PackageEventId = packageEventId,
                AdditionalRequest = additionalRequest,
                EventDate = eventDate,
                Status = "waiting validation",
                CreatedAt = DateTime.Now
            };

            await _orderRepo.Create(order);

            TempData["SuccessMessage"] = "Pesanan berhasil dibuat!";
            return RedirectToAction("Order");
        }


        // Tampilkan pesanan customer
        public async Task<IActionResult> Order(string? search, string? status)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (userId == null)
                return RedirectToAction("Login", "User");

            // Ambil semua order milik user
            var orders = await _orderRepo.Get(Guid.Parse(userId));

            if (!string.IsNullOrWhiteSpace(search))
            {
                orders = orders
                    .Where(o => o.PackageName.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // 🔽 Filter Status
            if (!string.IsNullOrWhiteSpace(status) && status != "all")
            {
                orders = orders.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(orders);
        }

        public async Task<IActionResult> DetailOrder(Guid id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "User");

            var order = await _orderRepo.GetById(id);

            if (order == null || order.UserId != Guid.Parse(userId))
                return NotFound();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmClient(Guid orderId)
        {
            var success = await _orderRepo.ConfirmOrder(orderId);

            if (success)
                TempData["SuccessMessage"] = "Pesanan berhasil dikonfirmasi!";
            else
                TempData["ErrorMessage"] = "Gagal melakukan konfirmasi.";

            return RedirectToAction("DetailOrder", new { id = orderId });
        }



    }
}
