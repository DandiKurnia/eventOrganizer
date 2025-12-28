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

            using var conn = context.CreateConnection();
            return await conn.QueryAsync<VendorModel>(sql);
        }


        public async Task<VendorModel> GetById(Guid vendorId)
        {
            var sql = "SELECT * FROM Vendor WHERE VendorId = @VendorId";
            using var connection = context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<VendorModel>(sql, new { VendorId = vendorId });
        }

        public async Task<VendorModel?> GetVendorByUserId(Guid userId)
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
        WHERE v.UserId = @UserId";

            using var conn = context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<VendorModel>(sql, new { UserId = userId });
        }


        public async Task Add(VendorModel vendor)
        {
            using var conn = context.CreateConnection();
            using var trans = conn.BeginTransaction();

            try
            {
                var vendorSql = @"
        INSERT INTO Vendor (VendorId, UserId, CompanyName, Status, Address, CreatedAt)
        VALUES (@VendorId, @UserId, @CompanyName, @Status, @Address, GETDATE())";

                await conn.ExecuteAsync(vendorSql, vendor, trans);

                if (vendor.SelectedCategoryIds.Any())
                {
                    var categorySql = @"
                INSERT INTO VendorCategory (VendorCategoryId, VendorId, CategoryId)
                VALUES (NEWID(), @VendorId, @CategoryId)";

                    foreach (var categoryId in vendor.SelectedCategoryIds)
                    {
                        await conn.ExecuteAsync(categorySql, new
                        {
                            VendorId = vendor.VendorId,
                            CategoryId = categoryId
                        }, trans);
                    }
                }

                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw;
            }
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

        public async Task<IEnumerable<VendorCategoryModel>> GetVendorCategories(Guid vendorId)
        {
            var sql = @"
        SELECT 
            vc.VendorCategoryId,
            vc.VendorId,
            vc.CategoryId,
            c.CategoryName
        FROM VendorCategory vc
        INNER JOIN Category c ON vc.CategoryId = c.CategoryId
        WHERE vc.VendorId = @VendorId";

            using var conn = context.CreateConnection();
            return await conn.QueryAsync<VendorCategoryModel>(sql, new { VendorId = vendorId });
        }

        public async Task AddVendorCategories(Guid vendorId, List<Guid> categoryIds)
        {
            using var conn = context.CreateConnection();
            conn.Open(); // 🔥 WAJIB

            using var trans = conn.BeginTransaction();

            try
            {
                // 🔥 hapus dulu kategori lama (untuk edit)
                var deleteSql = "DELETE FROM VendorCategory WHERE VendorId = @VendorId";
                await conn.ExecuteAsync(deleteSql, new { VendorId = vendorId }, trans);

                // 🔥 insert ulang
                var insertSql = @"
            INSERT INTO VendorCategory (VendorCategoryId, VendorId, CategoryId)
            VALUES (NEWID(), @VendorId, @CategoryId)";

                foreach (var categoryId in categoryIds)
                {
                    await conn.ExecuteAsync(insertSql, new
                    {
                        VendorId = vendorId,
                        CategoryId = categoryId
                    }, trans);
                }

                trans.Commit();
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }

        public async Task UpdateVendorInfo(VendorModel model)
        {
            const string sql = @"
        UPDATE Vendor
        SET CompanyName = @CompanyName,
            Address = @Address
        WHERE VendorId = @VendorId";

            using var conn = context.CreateConnection();
            await conn.ExecuteAsync(sql, model);
        }

        public async Task UpdateUserPhone(Guid userId, string? phoneNumber)
        {
            const string sql = @"
        UPDATE Users
        SET PhoneNumber = @PhoneNumber
        WHERE UserId = @UserId";

            using var conn = context.CreateConnection();
            await conn.ExecuteAsync(sql, new
            {
                UserId = userId,
                PhoneNumber = phoneNumber
            });
        }


        public async Task<VendorDashboardViewModel> GetVendorDashboard(Guid vendorId)
        {
            using var conn = context.CreateConnection();

            var sql = @"
    SELECT
        (SELECT COUNT(*) 
         FROM VendorConfirmation 
         WHERE VendorId = @VendorId) AS TotalOrders,

        (SELECT COUNT(*) 
         FROM VendorConfirmation 
         WHERE VendorId = @VendorId
         AND VendorStatus = 'waiting') AS WaitingConfirmation,

        (SELECT COUNT(*) 
         FROM VendorConfirmation 
         WHERE VendorId = @VendorId
         AND VendorStatus = 'confirmed') AS AcceptedOrders,

        (SELECT COUNT(*) 
         FROM VendorConfirmation 
         WHERE VendorId = @VendorId
         AND VendorStatus IN ('rejected','closed')) AS RejectedOrders,

        (SELECT ISNULL(SUM(ActualPrice),0)
         FROM VendorConfirmation
         WHERE VendorId = @VendorId
         AND VendorStatus = 'confirmed') AS TotalRevenue,

        (SELECT ISNULL(SUM(ActualPrice),0)
         FROM VendorConfirmation
         WHERE VendorId = @VendorId
         AND VendorStatus = 'confirmed'
         AND MONTH(CreatedAt) = MONTH(GETDATE())
         AND YEAR(CreatedAt) = YEAR(GETDATE())) AS MonthlyRevenue,

        (SELECT COUNT(*)
         FROM VendorConfirmation vc
         JOIN Orders o ON vc.OrderId = o.OrderId
         WHERE vc.VendorId = @VendorId
         AND o.EventDate BETWEEN GETDATE() AND DATEADD(DAY,7,GETDATE())) AS UpcomingEvents,

        (SELECT Status 
         FROM Vendor 
         WHERE VendorId = @VendorId) AS VendorStatus
    ";

            return await conn.QuerySingleAsync<VendorDashboardViewModel>(
                sql, new { VendorId = vendorId }
            );
        }




    }
}
