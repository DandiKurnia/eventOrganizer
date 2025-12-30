using Models;
namespace StaffEventOrganizer.Interface
{
    public interface IPackageEvent
    {
        Task<IEnumerable<PackageEventModel>> Get();
        Task<PackageEventModel> GetById(Guid id);

        Task<PackageEventModel> Add(PackageEventModel model);
        Task<PackageEventModel> Update(PackageEventModel model);
        Task Delete(Guid id);
        Task<IEnumerable<CategoryModel>> GetCategories(Guid packageEventId);

        Task UpdateCategories(Guid packageEventId, List<Guid> categoryIds);
    }
}
