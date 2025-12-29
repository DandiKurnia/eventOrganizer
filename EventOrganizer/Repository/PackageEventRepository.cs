using Dapper;
using EventOrganizer.DBContext;
using EventOrganizer.Interface;
using Models;

namespace EventOrganizer.Repository
{
    public class PackageEventRepository : IPackageEvent
    {
        private readonly DapperDbContext context;
        public PackageEventRepository(DapperDbContext context)
        {
            this.context = context;
        }

        public async Task<PackageEventModel> GetById(Guid id)
        {
            var sql = "SELECT * FROM eventPackage WHERE PackageEventId = @PackageEventId";
            using var connection = context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<PackageEventModel>(sql, new { PackageEventId = id });
        }

        public async Task<IEnumerable<PackageEventModel>> Get(string? search, Guid? categoryId)
        {
            var sql = @"
    SELECT DISTINCT p.*
    FROM eventPackage p
    LEFT JOIN PackageCategory pc ON p.PackageEventId = pc.PackageEventId
    WHERE (@Search IS NULL OR p.PackageName LIKE '%' + @Search + '%')
      AND (@CategoryId IS NULL OR pc.CategoryId = @CategoryId)
    ";

            using var conn = context.CreateConnection();
            return await conn.QueryAsync<PackageEventModel>(
                sql,
                new { Search = search, CategoryId = categoryId }
            );
        }



    }
}
