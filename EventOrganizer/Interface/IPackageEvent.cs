using Models;

namespace EventOrganizer.Interface
{
    public interface IPackageEvent
    {
        Task<IEnumerable<PackageEventModel>> Get(string? search, Guid? categoryId);
        Task<PackageEventModel?> GetById(Guid id);
    }

}
