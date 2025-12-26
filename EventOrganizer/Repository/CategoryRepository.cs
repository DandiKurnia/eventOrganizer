using Dapper;
using EventOrganizer.DBContext;
using EventOrganizer.Interface;
using Models;

namespace EventOrganizer.Repository
{
    public class CategoryRepository : ICategory
    {
        private readonly DapperDbContext _context;

        public CategoryRepository(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryModel>> GetAll()
        {
            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<CategoryModel>(
                "SELECT * FROM Category ORDER BY CategoryName"
            );
        }
    }
}
