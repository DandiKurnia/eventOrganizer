using StaffEventOrganizer.DBContext;
using StaffEventOrganizer.Interface;
using Models;
using Dapper;

namespace StaffEventOrganizer.Repository
{
    public class PackagePhotoRepository : IPackagePhoto
    {
        private readonly DapperContext _context;

        public PackagePhotoRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PackagePhoto>> GetByPackageId(Guid packageEventId)
        {
            var sql = @"SELECT PhotoId, PackageEventId, PhotoUrl, Foto, FotoContentType, CreatedAt
                        FROM PackagePhoto
                        WHERE PackageEventId = @PackageEventId";

            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<PackagePhoto>(sql, new { PackageEventId = packageEventId });
        }

        public async Task AddRange(IEnumerable<PackagePhoto> photos)
        {
            var sql = @"INSERT INTO PackagePhoto (PhotoId, PackageEventId, PhotoUrl, Foto, FotoContentType, CreatedAt)
                        VALUES (@PhotoId, @PackageEventId, @PhotoUrl, @Foto, @FotoContentType, @CreatedAt)";
            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, photos);
        }

        public async Task DeletePhoto(Guid photoId)
        {
            var sql = "DELETE FROM PackagePhoto WHERE PhotoId = @PhotoId";
            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new { PhotoId = photoId });
        }

        public async Task DeleteByPackageId(Guid packageEventId)
        {
            var sql = "DELETE FROM PackagePhoto WHERE PackageEventId = @PackageEventId";
            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new { PackageEventId = packageEventId });
        }

        public async Task<PackagePhoto?> GetByPhotoId(Guid photoId)
        {
            var sql = "SELECT * FROM packagePhoto WHERE PhotoId = @PhotoId";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<PackagePhoto>(sql, new { PhotoId = photoId });
        }
    }
}
