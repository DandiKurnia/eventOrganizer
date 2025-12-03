using Models;

namespace AdminEventOrganizer.Interface
{
    public interface IPackagePhoto
    {
        Task<IEnumerable<PackagePhoto>> GetByPackageId(Guid packageEventId);
        Task AddRange(IEnumerable<PackagePhoto> photos);
        Task DeleteByPackageId(Guid packageEventId);

        Task DeletePhoto(Guid photoId);
        Task<PackagePhoto?> GetByPhotoId(Guid photoId);
    }
}
