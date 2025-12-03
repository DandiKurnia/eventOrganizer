using AdminEventOrganizer.DBContext;
using AdminEventOrganizer.Interface;
using Dapper;
using Models;

namespace AdminEventOrganizer.Repository
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

        public async Task AddRange(IEnumerable<PackagePhoto> photos)
        {
            var sql = @"INSERT INTO packagePhoto (PhotoId, PackageEventId, PhotoUrl, Foto, FotoContentType, CreatedAt)
                VALUES (@PhotoId, @PackageEventId, @PhotoUrl, @Foto, @FotoContentType, @CreatedAt)";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, photos);
        }


        public async Task DeleteByPackageId(Guid packageEventId)
        {
            var sql = "DELETE FROM packagePhoto WHERE PackageEventId = @PackageEventId";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new { PackageEventId = packageEventId });
        }

        public async Task DeletePhoto(Guid photoId)
        {
            var sql = "DELETE FROM packagePhoto WHERE PhotoId = @PhotoId";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new { PhotoId = photoId });
        }

        public async Task<PackagePhoto?> GetByPhotoId(Guid photoId)
        {
            var sql = "SELECT * FROM packagePhoto WHERE PhotoId = @PhotoId";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<PackagePhoto>(sql, new { PhotoId = photoId });
        }

    }
}
