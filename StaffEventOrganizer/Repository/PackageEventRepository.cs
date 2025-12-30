using StaffEventOrganizer.DBContext;
using StaffEventOrganizer.Interface;
using Models;
using Dapper;

namespace StaffEventOrganizer.Repository
{
    public class PackageEventRepository : IPackageEvent
    {
        private readonly DapperContext context;
        public PackageEventRepository(DapperContext context)
        {
            this.context = context;
        }

        public async Task<PackageEventModel> GetById(Guid id)
        {
            var sql = "SELECT * FROM eventPackage WHERE PackageEventId = @PackageEventId";
            using var connection = context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<PackageEventModel>(sql, new { PackageEventId = id });
        }

        public async Task<IEnumerable<PackageEventModel>> Get()
        {
            var connection = context.CreateConnection();
            var sql = "SELECT * FROM eventPackage";

            try
            {
                var result = await connection.QueryAsync<PackageEventModel>(sql);
                return result;
            }
            catch(Exception ex)
            {
                throw new Exception($"Error fetching package data: {ex.Message}", ex);

            }

        }

        public async Task<PackageEventModel> Add(PackageEventModel model)
        {
            model.PackageEventId = Guid.NewGuid();
            var sql = @"INSERT INTO eventPackage (PackageEventId, PackageName, Description, BasePrice)
                        VALUES (@PackageEventId, @PackageName, @Description, @BasePrice)";
            using var connection = context.CreateConnection();
            await connection.ExecuteAsync(sql, model);
            return model;
        }

        public async Task<PackageEventModel> Update(PackageEventModel model)
        {
            var sql = @"UPDATE eventPackage 
                SET PackageName = @PackageName, 
                    Description = @Description, 
                    BasePrice = @BasePrice,
                    [Status] = @Status
                WHERE PackageEventId = @PackageEventId";

            using var connection = context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                PackageName = model.PackageName,
                Description = model.Description,
                BasePrice = model.BasePrice,
                Status = model.Status,
                PackageEventId = model.PackageEventId
            });

            return model;
        }


        public async Task Delete(Guid id)
        {
            using var connection = context.CreateConnection();
            var sql = "DELETE FROM eventPackage WHERE PackageEventId = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<CategoryModel>> GetCategories(Guid packageEventId)
        {
            var sql = @"
    SELECT c.CategoryId, c.CategoryName, c.CreatedAt
    FROM PackageCategory pc
    JOIN Category c ON pc.CategoryId = c.CategoryId
    WHERE pc.PackageEventId = @PackageEventId
    ";

            using var conn = context.CreateConnection();
            return await conn.QueryAsync<CategoryModel>(
                sql,
                new { PackageEventId = packageEventId }
            );
        }


        public async Task UpdateCategories(Guid packageEventId, List<Guid> categoryIds)
        {
            using var conn = context.CreateConnection();
            conn.Open();
            using var trans = conn.BeginTransaction();

            try
            {
                // hapus lama
                await conn.ExecuteAsync(
                    "DELETE FROM PackageCategory WHERE PackageEventId = @PackageEventId",
                    new { PackageEventId = packageEventId },
                    trans
                );

                // insert baru
                const string insertSql = @"
            INSERT INTO PackageCategory
            (PackageCategoryId, PackageEventId, CategoryId)
            VALUES (NEWID(), @PackageEventId, @CategoryId)";

                foreach (var categoryId in categoryIds)
                {
                    await conn.ExecuteAsync(insertSql, new
                    {
                        PackageEventId = packageEventId,
                        CategoryId = categoryId
                    }, trans);
                }

                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }




    }
}
