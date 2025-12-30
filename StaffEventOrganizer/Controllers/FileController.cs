using Microsoft.AspNetCore.Mvc;
using StaffEventOrganizer.Interface;

namespace StaffEventOrganizer.Controllers
{
    public class FileController : Controller
    {
        private readonly IPackageEvent _packageEventRepository;
        private readonly IPackagePhoto _packagePhotoRepository;

        public FileController(
            IPackageEvent packageEventRepository,
            IPackagePhoto packagePhotoRepository)
        {
            _packageEventRepository = packageEventRepository;
            _packagePhotoRepository = packagePhotoRepository;
        }
        public async Task<IActionResult> getFile(Guid id)
        {
            var data = await _packagePhotoRepository.GetByPhotoId(id);

            if (data == null)
                return NotFound();

            return File(data.Foto, data.FotoContentType ?? "application/octet-stream");
        }

    }
}
