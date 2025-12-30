using Models;

namespace StaffEventOrganizer.Interface
{
    public interface IPackagePhoto
    {
        Task<IEnumerable<PackagePhoto>> GetByPackageId(Guid packageEventId);
        Task AddRange(IEnumerable<PackagePhoto> photos);
        Task DeletePhoto(Guid photoId);
        Task DeleteByPackageId(Guid packageEventId);
        Task<PackagePhoto?> GetByPhotoId(Guid photoId);

    }
}
