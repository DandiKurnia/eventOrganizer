using EventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EventOrganizer.Controllers
{
    public class StaffController : Controller
    {
        private readonly IOrder _orderRepo;
        private readonly IVendor _vendorRepo;
        private readonly IVendorConfirmation _vendorConfirmRepo;

        public StaffController(IOrder orderRepo, IVendor vendorRepo, IVendorConfirmation vendorConfirmRepo)
        {
            _orderRepo = orderRepo;
            _vendorRepo = vendorRepo;
            _vendorConfirmRepo = vendorConfirmRepo;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Role") != "Staff")
                return RedirectToAction("Login", "User");

            return View();
        }

        public async Task<IActionResult> Pemesanan(string? search, int page = 1)
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
                return RedirectToAction(nameof(Pemesanan));

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
            var confirmations = await _vendorConfirmRepo.GetByOrderId(id);
            ViewBag.VendorConfirmations = confirmations?.ToList() ?? new List<VendorConfirmationModel>();

            return View(order);
        }


        [HttpPost]
        public async Task<IActionResult> SendVendor(Guid orderId, Guid vendorId)
        {
            var request = new VendorConfirmationModel
            {
                VendorConfirmationId = Guid.NewGuid(),
                OrderId = orderId,
                VendorId = vendorId,
                ActualPrice = 0,
                Notes = "",
                VendorStatus = "pending_vendor",
                CreatedAt = DateTime.Now
            };

            await _vendorConfirmRepo.Create(request);
            await _orderRepo.UpdateStatus(orderId, "vendor_selection");

            TempData["Success"] = "Request dikirim ke vendor.";
            return RedirectToAction("Edit", new { id = orderId });
        }

        public async Task<IActionResult> Confirm(Guid confirmationId)
        {
            var confirmation = await _vendorConfirmRepo.GetById(confirmationId);
            if (confirmation == null) return NotFound();

            await _vendorConfirmRepo.UpdateStatus(confirmationId, "vendor_confirmed");
            await _vendorConfirmRepo.CloseOtherVendors(confirmation.OrderId, confirmation.VendorId);

            await _orderRepo.UpdateStatus(confirmation.OrderId, "vendor_confirmed");

            TempData["SuccessMessage"] = "Vendor berhasil dikonfirmasi.";
            return RedirectToAction("Edit", new { id = confirmation.OrderId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid orderId, string status)
        {
            await _orderRepo.UpdateStatus(orderId, status);

            TempData["SuccessMessage"] = "Status order berhasil diperbarui.";
            return RedirectToAction("Edit", new { id = orderId });
        }

    }
}
