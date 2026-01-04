using AdminEventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AdminEventOrganizer.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrder _orderRepo;
        private readonly IVendor _vendorRepo;

        public OrderController(IOrder orderRepository, IVendor vendorRepository)
        {
            _orderRepo = orderRepository;
            _vendorRepo = vendorRepository;
        }

        public async Task<IActionResult> Index(string? search, int page = 1)
        {

            var orders = await _orderRepo.GetAll();

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

        public async Task<IActionResult> Detail(Guid id)
        {
            var order = await _orderRepo.GetById(id);
            if (order == null)
                return RedirectToAction(nameof(Index));

            // Ambil vendor confirmation untuk ditampilkan di Detail
            var confirmations = await _vendorRepo.GetByOrderId(id);
            ViewBag.VendorConfirmations = confirmations?.ToList() ?? new List<VendorConfirmationModel>();

            return View(order);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var order = await _orderRepo.GetById(id);
            if (order == null)
                return NotFound();

            // 🔥 Ambil vendor available
            ViewBag.AvailableVendors = await _vendorRepo.GetAvailableVendors();

            // 🔥 Ambil vendor confirmation (TIDAK BOLEH NULL)
            var confirmations = await _vendorRepo.GetByOrderId(id);
            ViewBag.VendorConfirmations = confirmations?.ToList() ?? new List<VendorConfirmationModel>();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid orderId, string status)
        {
            await _orderRepo.UpdateStatus(orderId, status);
            TempData["SuccessMessage"] = "Status pemesanan berhasil diperbarui";
            return RedirectToAction(nameof(Edit), new { id = orderId });
        }

        [HttpPost]
        public async Task<IActionResult> SendVendor(Guid orderId, Guid packageEventId)
        {
            try
            {
                await _vendorRepo.SendVendorRequestByPackage(orderId, packageEventId);
                TempData["SuccessMessage"] =
                    "Request berhasil dikirim ke semua vendor yang sesuai kategori.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Edit), new { id = orderId });
        }



    }
}
