using StaffEventOrganizer.Interface;
using Models;
using Dapper;
using StaffEventOrganizer.DBContext;

namespace StaffEventOrganizer.Repository
{
    public class CategoryRepository : ICategory
    {
        private readonly DapperContext _context;

        public CategoryRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryModel>> GetAll()
        {
            const string sql = @"
                SELECT CategoryId, CategoryName, CreatedAt
                FROM Category
                ORDER BY CreatedAt DESC";

            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<CategoryModel>(sql);
        }

        public async Task<CategoryModel?> GetById(Guid id)
        {
            const string sql = @"
                SELECT CategoryId, CategoryName, CreatedAt
                FROM Category
                WHERE CategoryId = @Id";

            using var conn = _context.CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<CategoryModel>(sql, new { Id = id });
        }

        public async Task Create(CategoryModel model)
        {
            const string sql = @"
                INSERT INTO Category (CategoryId, CategoryName, CreatedAt)
                VALUES (NEWID(), @CategoryName, GETDATE())";

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, model);
        }

        public async Task Update(CategoryModel model)
        {
            const string sql = @"
                UPDATE Category
                SET CategoryName = @CategoryName
                WHERE CategoryId = @CategoryId";

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, model);
        }

        public async Task Delete(Guid id)
        {
            const string sql = "DELETE FROM Category WHERE CategoryId = @Id";

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
