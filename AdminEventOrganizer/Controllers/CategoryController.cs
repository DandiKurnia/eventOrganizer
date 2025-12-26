using AdminEventOrganizer.Interface;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AdminEventOrganizer.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategory _categoryRepo;

        public CategoryController(ICategory categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        // GET: /Category
        public async Task<IActionResult> Index(string? search)
        {
            var data = await _categoryRepo.GetAll();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                data = data.Where(c =>
                    c.CategoryName.ToLower().Contains(search)
                );
            }

            ViewBag.Search = search;
            return View(data);
        }

        // GET: /Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Category/Create
        [HttpPost]
        public async Task<IActionResult> Create(CategoryModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryRepo.Create(model);
            TempData["SuccessMessage"] = "Kategori berhasil ditambahkan";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Category/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await _categoryRepo.GetById(id);
            if (data == null)
                return NotFound();

            return View(data);
        }

        // POST: /Category/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(CategoryModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryRepo.Update(model);
            TempData["SuccessMessage"] = "Kategori berhasil diperbarui";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Category/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categoryRepo.Delete(id);
            TempData["SuccessMessage"] = "Kategori berhasil dihapus";
            return RedirectToAction(nameof(Index));
        }
    }
}
