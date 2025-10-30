using AdminEventOrganizer.Filters;
using AdminEventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AdminEventOrganizer.Controllers
{
    [TypeFilter(typeof(AuthorizationFilter))]
    public class PackageEventController : Controller
    {
        private readonly IPackageEvent _packageEventRepository;

        public PackageEventController(IPackageEvent packageEventRepository)
        {
            this._packageEventRepository = packageEventRepository;
        }
        public async Task <IActionResult> Index()
        {
            var PackageEvenets = await _packageEventRepository.Get();
            return View(PackageEvenets);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PackageEventModel model)
        {
            if (ModelState.IsValid)
            {
                await _packageEventRepository.Add(model);
                TempData["SuccessMessage"] = "Paket Event berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var package = await _packageEventRepository.GetById(id);
            if (package == null)
            {
                TempData["ErrorMessage"] = "Data tidak ditemukan!";
                return RedirectToAction(nameof(Index));
            }
            return View(package);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PackageEventModel formModel)
        {
            if (!ModelState.IsValid)
                return View(formModel);

            var existing = await _packageEventRepository.GetById(id);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "Data tidak ditemukan!";
                return RedirectToAction(nameof(Index));
            }

            existing.PackageName = formModel.PackageName;
            existing.Description = formModel.Description;
            existing.BasePrice = formModel.BasePrice;
            existing.Status = formModel.Status;

            await _packageEventRepository.Update(existing);
            TempData["SuccessMessage"] = "Paket event berhasil diperbarui!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _packageEventRepository.Delete(id);
            TempData["SuccessMessage"] = "Paket event berhasil dihapus!";
            return RedirectToAction("Index");
        }

    }
}
