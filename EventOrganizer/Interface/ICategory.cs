using Models;

namespace EventOrganizer.Interface
{
    public interface ICategory
    {
        Task<IEnumerable<CategoryModel>> GetAll();
    }
}
