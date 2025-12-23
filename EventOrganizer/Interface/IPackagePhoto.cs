using Models;

namespace EventOrganizer.Interface
{
    public interface IPackagePhoto
    {
        Task<PackagePhoto?> GetByPhotoId(Guid photoId);

        Task<IEnumerable<PackagePhoto>> GetByPackageId(Guid packageEventId);
    }
}
