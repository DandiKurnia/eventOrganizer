using Microsoft.AspNetCore.Mvc;
using Models;
using EventOrganizer.Interface;
using EventOrganizer.Repository;


namespace EventOrganizer.Controllers
{
    public class FileController : Controller
    {
        private readonly IPackagePhoto _packagePhotoRepository;
        private readonly IPackageEvent _packageEventRepository;

        public FileController(IPackagePhoto packagePhotoRepository, IPackageEvent packageEventRepository)
        {
            _packagePhotoRepository = packagePhotoRepository;
            _packageEventRepository = packageEventRepository;
        }

        public async Task<IActionResult> getFile(Guid id)
        {
            var data = await _packagePhotoRepository.GetByPhotoId(id);
            if (data == null)
            {
                return NotFound();
            }
            return File(data.Foto, data.FotoContentType ?? "application/octet-stream");
        }

    }
}
