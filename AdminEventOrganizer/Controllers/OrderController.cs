using AdminEventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AdminEventOrganizer.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrder _orderRepo;
        private readonly IVendor _vendorRepo;
        private readonly IEmailService _emailService;

        public OrderController(IOrder orderRepository, IVendor vendorRepository, IEmailService emailService)
        {
            _orderRepo = orderRepository;
            _vendorRepo = vendorRepository;
            _emailService = emailService;
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

            // Ambil vendor yang sesuai dengan kategori paket
            ViewBag.AvailableVendors = await _vendorRepo.GetAvailableVendorsByPackage(order.PackageEventId, id);

            // Ambil vendor confirmation (TIDAK BOLEH NULL)
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
                // 1. Ambil list vendor yang akan dikirimi request (sebelum insert, agar NOT EXISTS masih valid)
                // Pastikan GetAvailableVendorsByPackage sudah mengembalikan Email (sudah kita update di Repo)
                var targetVendors = await _vendorRepo.GetAvailableVendorsByPackage(packageEventId, orderId);

                if (!targetVendors.Any())
                {
                    TempData["ErrorMessage"] = "Tidak ada vendor tersedia untuk dikirim request.";
                    return RedirectToAction(nameof(Edit), new { id = orderId });
                }

                // 2. Insert ke Database
                await _vendorRepo.SendVendorRequestByPackage(orderId, packageEventId);

                // 3. Kirim Email ke masing-masing vendor
                var order = await _orderRepo.GetById(orderId);
                if (order != null)
                {
                    foreach (var vendor in targetVendors)
                    {
                        if (!string.IsNullOrEmpty(vendor.Email))
                        {
                            await _emailService.SendVendorRequestEmailAsync(
                                vendor.Email,
                                vendor.CompanyName,
                                order.ClientName,
                                order.PackageName,
                                order.EventDate
                            );
                        }
                    }
                }

                TempData["SuccessMessage"] =
                    $"Request berhasil dikirim dan email notifikasi telah dikirim ke {targetVendors.Count()} vendor.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Terjadi kesalahan: " + ex.Message;
            }

            return RedirectToAction(nameof(Edit), new { id = orderId });
        }



    }
}
