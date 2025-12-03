using AdminEventOrganizer.DBContext;
using AdminEventOrganizer.Interface;
using Models;
using Dapper;

namespace AdminEventOrganizer.Repository
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
            catch(Exception ex)
            {
                throw new Exception($"Error fetching vendor data: {ex.Message}", ex);

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



    }
}
