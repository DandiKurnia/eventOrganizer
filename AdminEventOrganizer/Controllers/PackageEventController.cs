using AdminEventOrganizer.Interface;
using AdminEventOrganizer.Repository;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AdminEventOrganizer.Controllers
{
    public class PackageEventController : Controller
    {
        private readonly IPackageEvent _packageEventRepository;
        private readonly IPackagePhoto _packagePhotoRepository;

        public PackageEventController(
            IPackageEvent packageEventRepository,
            IPackagePhoto packagePhotoRepository)
        {
            _packageEventRepository = packageEventRepository;
            _packagePhotoRepository = packagePhotoRepository;
        }

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

            var data = packages.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;

            return View(data);
        }





        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PackageEventModel model, List<IFormFile> PhotoFiles)
        {
            if (ModelState.IsValid)
            {
                var addedPackage = await _packageEventRepository.Add(model);
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                // 3️⃣ Simpan file dan catat URL-nya
                var photos = new List<PackagePhoto>();

                foreach (var file in PhotoFiles)
                {
                    if (file != null && file.Length > 0)
                    {
                        string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);

                        // Simpan file ke server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        byte[] fileData;
                        using (var ms = new MemoryStream()) { await file.CopyToAsync(ms); fileData = ms.ToArray(); }

                        var contentType = file.ContentType;
                        // Simpan path ke database (relatif ke wwwroot)
                        photos.Add(new PackagePhoto
                        {
                            PhotoId = Guid.NewGuid(),
                            PackageEventId = addedPackage.PackageEventId,
                            PhotoUrl = $"/uploads/{uniqueFileName}",  // nanti bisa ditampilkan langsung di <img src="">
                            CreatedAt = DateTime.Now,
                            Foto = fileData,
                            FotoContentType = contentType,
                        });
                    }
                }

                if (photos.Any())
                    await _packagePhotoRepository.AddRange(photos);

                TempData["SuccessMessage"] = "Paket Event berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var package = await _packageEventRepository.GetById(id);
            if (package == null)
            {
                TempData["ErrorMessage"] = "Data tidak ditemukan!";
                return RedirectToAction(nameof(Index));
            }

            // Ambil semua foto dari repository foto
            var photos = await _packagePhotoRepository.GetByPackageId(id);
            package.Photos = photos.ToList();

            return View(package);
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

            var photos = await _packagePhotoRepository.GetByPackageId(id);
            package.Photos = photos.ToList();
            return View(package);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PackageEventModel model, List<IFormFile> PhotoFiles)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = await _packageEventRepository.GetById(id);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "Data tidak ditemukan!";
                return RedirectToAction(nameof(Index));
            }

            // Update data utama
            existing.PackageName = model.PackageName;
            existing.Description = model.Description;
            existing.BasePrice = model.BasePrice;
            existing.Status = model.Status;
            await _packageEventRepository.Update(existing);

            // Upload foto baru jika ada
            if (PhotoFiles != null && PhotoFiles.Any())
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var photos = new List<PackagePhoto>();

                foreach (var file in PhotoFiles)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    // 1️⃣ SIMPAN FILE KE FOLDER
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    byte[] fileData;
                    using (var ms = new MemoryStream())
                    using (var input = file.OpenReadStream())
                    {
                        await input.CopyToAsync(ms);   // baca stream dari awal
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
            return RedirectToAction(nameof(Index), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto(Guid photoId, Guid packageEventId)
        {
            // hapus satu foto
            await _packagePhotoRepository.DeletePhoto(photoId);
            TempData["SuccessMessage"] = "Foto berhasil dihapus!";
            return RedirectToAction(nameof(Edit), new { id = packageEventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                // Ambil semua foto terkait package
                var photos = await _packagePhotoRepository.GetByPackageId(id);

                // Hapus file fisik di wwwroot/uploads
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                foreach (var photo in photos)
                {
                    string filePath = Path.Combine(uploadFolder, Path.GetFileName(photo.PhotoUrl));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Hapus semua data foto di database
                await _packagePhotoRepository.DeleteByPackageId(id);

                // Hapus data package
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
