using Models;

namespace EventOrganizer.Interface
{
    public interface IPackageEvent
    {
        Task<IEnumerable<PackageEventModel>> Get();
        Task<PackageEventModel> GetById(Guid id);

    }
}
