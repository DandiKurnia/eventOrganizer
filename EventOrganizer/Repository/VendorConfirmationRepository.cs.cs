using Dapper;
using EventOrganizer.DBContext;
using EventOrganizer.Interface;
using Models;

namespace EventOrganizer.Repository
{
    public class VendorConfirmationRepository : IVendorConfirmation
    {
        private readonly DapperDbContext _context;

        public VendorConfirmationRepository(DapperDbContext context)
        {
            _context = context;
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

        public async Task<VendorConfirmationModel?> GetById(Guid id)
        {
            var sql = @"
        SELECT
            vc.VendorConfirmationId,
            vc.OrderId,
            vc.VendorId,
            vc.ActualPrice,
            vc.Notes,
            vc.VendorStatus,
            vc.CreatedAt,

            v.CompanyName        AS VendorName,
            p.PackageName        AS PackageName,
            p.BasePrice          AS BasePrice,
            o.AdditionalRequest  AS AdditionalRequest,
            o.EventDate          AS EventDate

        FROM VendorConfirmation vc
        JOIN Vendor v ON vc.VendorId = v.VendorId
        JOIN Orders o ON vc.OrderId = o.OrderId
        JOIN eventPackage p ON o.PackageEventId = p.PackageEventId
        WHERE vc.VendorConfirmationId = @Id";

            using var conn = _context.CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<VendorConfirmationModel>(
                sql,
                new { Id = id }
            );
        }




        public async Task<VendorConfirmationModel> Create(VendorConfirmationModel model)
        {
            using var conn = _context.CreateConnection();

            var checkSql = @"
                SELECT COUNT(*) 
                FROM VendorConfirmation
                WHERE OrderId = @OrderId AND VendorStatus = 'vendor_confirmed'";

            var confirmed = await conn.ExecuteScalarAsync<int>(checkSql, new { model.OrderId });

            if (confirmed > 0)
                throw new Exception("Vendor lain sudah melakukan konfirmasi.");

            var sql = @"
                INSERT INTO VendorConfirmation
                (VendorConfirmationId, OrderId, VendorId, ActualPrice, Notes, VendorStatus, CreatedAt)
                VALUES (@VendorConfirmationId, @OrderId, @VendorId, @ActualPrice, @Notes, @VendorStatus, @CreatedAt)";

            await conn.ExecuteAsync(sql, model);
            return model;
        }

        public async Task UpdateStatus(Guid vendorConfirmationId, string newStatus)
        {
            var sql = "UPDATE VendorConfirmation SET VendorStatus = @newStatus WHERE VendorConfirmationId = @id";

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new { id = vendorConfirmationId, newStatus });
        }

        public async Task<IEnumerable<VendorOrderViewModel>> GetOrdersForVendor(Guid vendorId)
        {
            var sql = @"
        SELECT
            vc.VendorConfirmationId,
            vc.OrderId,
            vc.ActualPrice,
            vc.VendorStatus,
            vc.CreatedAt,
            o.EventDate,
            u.Name AS ClientName,
            u.Email AS ClientEmail,
            p.PackageName
        FROM VendorConfirmation vc
        INNER JOIN Orders o ON vc.OrderId = o.OrderId
        INNER JOIN Users u ON o.UserId = u.UserId
        INNER JOIN eventPackage p ON o.PackageEventId = p.PackageEventId
        WHERE vc.VendorId = @VendorId
          AND vc.VendorStatus != 'closed'
        ORDER BY vc.CreatedAt DESC";

            using var conn = _context.CreateConnection();
            return await conn.QueryAsync<VendorOrderViewModel>(sql, new { VendorId = vendorId });
        }

        public async Task UpdateAccept(
    Guid vendorConfirmationId,
    decimal actualPrice,
    string? notes)
        {
            var sql = @"
        UPDATE VendorConfirmation
        SET
            ActualPrice = @ActualPrice,
            Notes = @Notes,
            VendorStatus = 'confirmed'
        WHERE VendorConfirmationId = @Id";

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new
            {
                Id = vendorConfirmationId,
                ActualPrice = actualPrice,
                Notes = notes
            });
        }

        public async Task CloseOtherVendors(Guid orderId, Guid vendorId)
        {
            var sql = @"
        UPDATE VendorConfirmation
        SET VendorStatus = 'closed'
        WHERE OrderId = @OrderId
          AND VendorId != @VendorId";

            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(sql, new
            {
                OrderId = orderId,
                VendorId = vendorId
            });
        }



    }
}
