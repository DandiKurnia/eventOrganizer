using Dapper;
using EventOrganizer.DBContext;
using EventOrganizer.Interface;
using Models;

namespace EventOrganizer.Repository
{
    public class PackagePhotoRepository : IPackagePhoto
    {
        private readonly DapperDbContext _context;

        public PackagePhotoRepository(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PackagePhoto>> GetByPackageId(Guid packageEventId)
        {
            var sql = "SELECT * FROM packagePhoto WHERE PackageEventId = @PackageEventId";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<PackagePhoto>(sql, new { PackageEventId = packageEventId });
        }

        public async Task<PackagePhoto?> GetByPhotoId(Guid photoId)
        {
            var sql = "SELECT * FROM packagePhoto WHERE PhotoId = @PhotoId";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<PackagePhoto>(sql, new { PhotoId = photoId });
        }

    }
}
