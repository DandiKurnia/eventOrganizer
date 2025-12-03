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

        public async Task<IEnumerable<PackageEventModel>> Get()
        {
            var connection = context.CreateConnection();
            var sql = "SELECT * FROM eventPackage";

            try
            {
                var result = await connection.QueryAsync<PackageEventModel>(sql);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching vendor data: {ex.Message}", ex);

            }

        }

    }
}
