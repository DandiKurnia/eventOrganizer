using AdminEventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AdminEventOrganizer.Controllers
{
    public class PackageEventController : Controller
    {
        private readonly IPackageEvent _packageEventRepository;
        private readonly IPackagePhoto _packagePhotoRepository;
        private readonly ICategory _categoryRepository; // 🔥 CATEGORY

        public PackageEventController(
            IPackageEvent packageEventRepository,
            IPackagePhoto packagePhotoRepository,
            ICategory categoryRepository)
        {
            _packageEventRepository = packageEventRepository;
            _packagePhotoRepository = packagePhotoRepository;
            _categoryRepository = categoryRepository;
        }

        // =====================================================
        // INDEX
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            const int pageSize = 10;
            var packages = await _packageEventRepository.Get();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                packages = packages.Where(p =>
                    p.PackageName.ToLower().Contains(search) ||
                    p.Description.ToLower().Contains(search) ||
                    p.Status.ToLower().Contains(search));
            }

            var totalItems = packages.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var data = packages.Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;

            return View(data);
        }

        // =====================================================
        // CREATE (GET)
        // =====================================================
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryRepository.GetAll(); // 🔥 CATEGORY
            return View();
        }

        // =====================================================
        // CREATE (POST)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PackageEventModel model, List<IFormFile> PhotoFiles)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryRepository.GetAll();
                return View(model);
            }

            // 1️⃣ SIMPAN PACKAGE
            var addedPackage = await _packageEventRepository.Add(model);

            // 🔥 SIMPAN RELASI CATEGORY
            if (model.SelectedCategoryIds.Any())
            {
                await _packageEventRepository.UpdateCategories(
                    addedPackage.PackageEventId,
                    model.SelectedCategoryIds
                );
            }

            // 2️⃣ UPLOAD FOTO (KODE KAMU TIDAK DIUBAH)
            string uploadFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "uploads");

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var photos = new List<PackagePhoto>();

            foreach (var file in PhotoFiles)
            {
                if (file != null && file.Length > 0)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    byte[] fileData;
                    using (var ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        fileData = ms.ToArray();
                    }

                    photos.Add(new PackagePhoto
                    {
                        PhotoId = Guid.NewGuid(),
                        PackageEventId = addedPackage.PackageEventId,
                        PhotoUrl = $"/uploads/{uniqueFileName}",
                        Foto = fileData,
                        FotoContentType = file.ContentType,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            if (photos.Any())
                await _packagePhotoRepository.AddRange(photos);

            TempData["SuccessMessage"] = "Paket Event berhasil ditambahkan!";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // DETAIL
        // =====================================================
        public async Task<IActionResult> Detail(Guid id)
        {
            var package = await _packageEventRepository.GetById(id);
            if (package == null)
            {
                TempData["ErrorMessage"] = "Data tidak ditemukan!";
                return RedirectToAction(nameof(Index));
            }

            package.Photos = (await _packagePhotoRepository.GetByPackageId(id)).ToList();
            package.Categories = (await _packageEventRepository.GetCategories(id)).ToList(); // 🔥 CATEGORY

            return View(package);
        }

        // =====================================================
        // EDIT (GET)
        // =====================================================
        public async Task<IActionResult> Edit(Guid id)
        {
            var package = await _packageEventRepository.GetById(id);
            if (package == null) return NotFound();

            package.Photos = (await _packagePhotoRepository.GetByPackageId(id)).ToList();

            var selected = await _packageEventRepository.GetCategories(id);
            package.SelectedCategoryIds = selected.Select(x => x.CategoryId).ToList();

            ViewBag.Categories = await _categoryRepository.GetAll();
            ViewBag.SelectedCategories = selected;

            return View(package);
        }

        // =====================================================
        // EDIT (POST)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PackageEventModel model, List<IFormFile> PhotoFiles)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryRepository.GetAll();
                return View(model);
            }

            var existing = await _packageEventRepository.GetById(id);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "Data tidak ditemukan!";
                return RedirectToAction(nameof(Index));
            }

            // UPDATE PACKAGE
            existing.PackageName = model.PackageName;
            existing.Description = model.Description;
            existing.BasePrice = model.BasePrice;
            existing.Status = model.Status;

            await _packageEventRepository.Update(existing);

            // 🔥 UPDATE CATEGORY
            await _packageEventRepository.UpdateCategories(
                existing.PackageEventId,
                model.SelectedCategoryIds
            );

            // UPLOAD FOTO BARU (KODE KAMU)
            if (PhotoFiles != null && PhotoFiles.Any())
            {
                string uploadFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "uploads");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var photos = new List<PackagePhoto>();

                foreach (var file in PhotoFiles)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    byte[] fileData;
                    using (var ms = new MemoryStream())
                    using (var input = file.OpenReadStream())
                    {
                        await input.CopyToAsync(ms);
                        fileData = ms.ToArray();
                    }

                    photos.Add(new PackagePhoto
                    {
                        PhotoId = Guid.NewGuid(),
                        PackageEventId = existing.PackageEventId,
                        PhotoUrl = $"/uploads/{uniqueFileName}",
                        Foto = fileData,
                        FotoContentType = file.ContentType,
                        CreatedAt = DateTime.Now
                    });
                }

                await _packagePhotoRepository.AddRange(photos);
            }

            TempData["SuccessMessage"] = "Paket event berhasil diperbarui!";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // DELETE FOTO
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> DeletePhoto(Guid photoId, Guid packageEventId)
        {
            await _packagePhotoRepository.DeletePhoto(photoId);
            TempData["SuccessMessage"] = "Foto berhasil dihapus!";
            return RedirectToAction(nameof(Edit), new { id = packageEventId });
        }

        // =====================================================
        // DELETE PACKAGE
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var photos = await _packagePhotoRepository.GetByPackageId(id);
                string uploadFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "uploads");

                foreach (var photo in photos)
                {
                    string filePath = Path.Combine(
                        uploadFolder,
                        Path.GetFileName(photo.PhotoUrl));

                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                await _packagePhotoRepository.DeleteByPackageId(id);
                await _packageEventRepository.Delete(id);

                TempData["SuccessMessage"] = "Paket event dan semua fotonya berhasil dihapus!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Gagal menghapus data: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
