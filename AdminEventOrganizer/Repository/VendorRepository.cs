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

        public async Task<IEnumerable<VendorModel>> GetAvailableVendors()
        {
            using var conn = _context.CreateConnection();
            var sql = "SELECT * FROM Vendor WHERE Status = 'available'";
            return await conn.QueryAsync<VendorModel>(sql);
        }

        public async Task<IEnumerable<VendorConfirmationModel>> GetByOrderId(Guid orderId)
        {
            var sql = @"
                SELECT vc.*, v.CompanyName AS VendorName
                FROM VendorConfirmation vc
                INNER JOIN Vendor v ON vc.VendorId = v.VendorId
                WHERE vc.OrderId = @OrderId
                ORDER BY vc.CreatedAt DESC";

            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<VendorConfirmationModel>(sql, new { OrderId = orderId });
        }

        public async Task SendVendorRequest(Guid orderId, Guid vendorId)
        {
            var sql = @"
        INSERT INTO VendorConfirmation
        (
            VendorConfirmationId,
            OrderId,
            VendorId,
            ActualPrice,
            VendorStatus,
            CreatedAt
        )
        VALUES
        (
            NEWID(),
            @OrderId,
            @VendorId,
            0,
            'pending_vendor',
            GETDATE()
        )";

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new
            {
                OrderId = orderId,
                VendorId = vendorId
            });
        }

        public async Task<IEnumerable<VendorCategoryModel>> GetVendorCategories(Guid vendorId)
        {
            const string sql = @"
        SELECT 
            vc.VendorCategoryId,
            vc.VendorId,
            vc.CategoryId,
            c.CategoryName
        FROM VendorCategory vc
        INNER JOIN Category c 
            ON vc.CategoryId = c.CategoryId
        WHERE vc.VendorId = @VendorId
        ORDER BY c.CategoryName";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<VendorCategoryModel>(sql, new
            {
                VendorId = vendorId
            });
        }



    }
}
