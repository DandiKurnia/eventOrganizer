using Dapper;
using EventOrganizer.DBContext;
using EventOrganizer.Interface;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventOrganizer.Repository
{
    public class VendorRepository : IVendor
    {
        private readonly DapperDbContext context;

        public VendorRepository(DapperDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<VendorModel>> Get()
        {
            var sql = "SELECT * FROM Vendor";
            using var connection = context.CreateConnection();
            return await connection.QueryAsync<VendorModel>(sql);
        }

        public async Task<VendorModel> GetById(Guid vendorId)
        {
            var sql = "SELECT * FROM Vendor WHERE VendorId = @VendorId";
            using var connection = context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<VendorModel>(sql, new { VendorId = vendorId });
        }

        public async Task<VendorModel> GetVendorByUserId(Guid userId)
        {
            var sql = "SELECT * FROM Vendor WHERE UserId = @UserId";
            using var connection = context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<VendorModel>(sql, new { UserId = userId });
        }

        public async Task Add(VendorModel vendor)
        {
            var sql = @"INSERT INTO Vendor (VendorId, UserId, CompanyName, Category, Email, Phone, Status, Address, CreatedAt)
                        VALUES (@VendorId, @UserId, @CompanyName, @Category, @Email, @Phone, @Status, @Address, @CreatedAt)";
            using var connection = context.CreateConnection();
            await connection.ExecuteAsync(sql, vendor);
        }

        public async Task UpdateStatus(Guid vendorId, string status)
        {
            var sql = "UPDATE Vendor SET Status = @Status WHERE VendorId = @VendorId";
            using var connection = context.CreateConnection();
            await connection.ExecuteAsync(sql, new { VendorId = vendorId, Status = status });
        }

        public async Task<IEnumerable<OrderModel>> GetOrders(Guid vendorId, string? search)
        {
            var sql = @"
        SELECT 
            o.OrderId,
            o.UserId,
            o.PackageEventId,
            o.AdditionalRequest,
            o.OrderDate,
            o.Status,
            o.CreatedAt,
            p.PackageName
        FROM Orders AS o
        INNER JOIN eventPackage AS p 
            ON o.PackageEventId = p.PackageEventId
        WHERE o.VendorId = @VendorId
        ORDER BY o.CreatedAt DESC";

            using var connection = context.CreateConnection();
            var data = await connection.QueryAsync<OrderModel>(sql, new { VendorId = vendorId });

            // Apply Search (C# filtering)
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                data = data.Where(o =>
                    o.PackageName.ToLower().Contains(search) ||
                    o.Status.ToLower().Contains(search));
            }

            return data;
        }

        public async Task<OrderModel?> GetOrderDetail(Guid orderId)
        {
            var sql = @"
        SELECT 
            o.OrderId,
            o.UserId,
            o.PackageEventId,
            o.AdditionalRequest,
            o.OrderDate,
            o.Status,
            o.CreatedAt,
            p.PackageName
        FROM Orders AS o
        INNER JOIN eventPackage AS p 
            ON o.PackageEventId = p.PackageEventId
        WHERE o.OrderId = @OrderId";

            using var connection = context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<OrderModel>(sql, new { OrderId = orderId });
        }

        public async Task<IEnumerable<VendorModel>> GetAvailableVendors()
        {
            using var conn = context.CreateConnection();
            var sql = "SELECT * FROM Vendor WHERE Status = 'available'";
            return await conn.QueryAsync<VendorModel>(sql);
        }


    }
}
