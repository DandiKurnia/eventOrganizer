using Models;
namespace AdminEventOrganizer.Interface
{
    public interface IPackageEvent
    {
        Task<IEnumerable<PackageEventModel>> Get();
        Task<PackageEventModel> Add(PackageEventModel model);
        Task<PackageEventModel> Update(PackageEventModel model);
        Task Delete(Guid id);
        Task<PackageEventModel> GetById(Guid id);
    }
}
