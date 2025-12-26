using Models;

namespace AdminEventOrganizer.Interface
{
    public interface ICategory
    {
        Task<IEnumerable<CategoryModel>> GetAll();
        Task<CategoryModel?> GetById(Guid id);

        Task Create(CategoryModel model);
        Task Update(CategoryModel model);
        Task Delete(Guid id);
    }
}
