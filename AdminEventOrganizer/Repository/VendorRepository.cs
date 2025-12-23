using AdminEventOrganizer.DBContext;
using AdminEventOrganizer.Interface;
using Dapper;
using Models;

namespace AdminEventOrganizer.Repository
{
    public class VendorRepository : IVendor
    {
        private readonly DapperDbContext _context;

        public VendorRepository(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VendorModel>> Get()
        {
            const string sql = @"
        SELECT 
            v.VendorId,
            v.UserId,
            v.CompanyName,
            v.Category,
            u.Email,
            u.PhoneNumber,
            v.Status,
            v.Address,
            v.CreatedAt
        FROM Vendor v
        LEFT JOIN Users u ON v.UserId = u.UserId
        ORDER BY v.CreatedAt DESC";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<VendorModel>(sql);
        }



        public async Task<VendorModel?> GetById(Guid id)
        {
            const string sql = @"
                SELECT 
                    v.VendorId,
                    v.UserId,
                    v.CompanyName,
                    v.Category,
                    u.Email,
                    u.PhoneNumber,
                    v.Status,
                    v.Address,
                    v.CreatedAt
                FROM Vendor v
                LEFT JOIN Users u ON v.UserId = u.UserId
                WHERE v.VendorId = @Id";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<VendorModel>(sql, new { Id = id });
        }



        public async Task UpdateStatus(Guid vendorId, string status)
        {
            const string sql = @"
                UPDATE Vendor
                SET Status = @Status
                WHERE VendorId = @VendorId";

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                VendorId = vendorId,
                Status = status
            });
        }
    }
}
