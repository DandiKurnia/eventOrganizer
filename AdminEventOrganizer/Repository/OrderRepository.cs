using AdminEventOrganizer.Interface;
using Dapper;
using AdminEventOrganizer.DBContext;
using Models;

namespace AdminEventOrganizer.Repository
{
    public class OrderRepository : IOrder
    {
        private readonly DapperDbContext context;

        public OrderRepository(DapperDbContext dbContext)
        {
            context = dbContext;
        }

        public async Task<IEnumerable<OrderModel>> GetAll()
        {
            var sql = @"
                SELECT 
                    o.OrderId,
                    o.UserId,
                    o.PackageEventId,
                    o.AdditionalRequest,
                    o.EventDate,
                    o.Status,
                    o.CreatedAt,
                    p.PackageName,
                    u.Name AS ClientName,
                    u.Email AS ClientEmail,
                    u.PhoneNumber AS ClientPhoneNumber
                FROM Orders o
                INNER JOIN eventPackage p ON o.PackageEventId = p.PackageEventId
                INNER JOIN Users u ON o.UserId = u.UserId
                ORDER BY o.CreatedAt DESC";

            using var connection = context.CreateConnection();
            return await connection.QueryAsync<OrderModel>(sql);
        }

        public async Task<OrderModel?> GetById(Guid orderId)
        {
            var sql = @"
        SELECT 
            o.OrderId,
            o.UserId,
            o.PackageEventId,
            o.AdditionalRequest,
            o.EventDate,
            o.Status,
            o.CreatedAt,
            p.PackageName,
            u.Name AS ClientName,
            u.Email AS ClientEmail,
            u.PhoneNumber AS ClientPhoneNumber
        FROM Orders o
        INNER JOIN eventPackage p ON o.PackageEventId = p.PackageEventId
        INNER JOIN Users u ON o.UserId = u.UserId
        WHERE o.OrderId = @OrderId";

            using var connection = context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<OrderModel>(
                sql, new { OrderId = orderId }
            );
        }


        public async Task<bool> ConfirmOrder(Guid orderId)
        {
            var sql = @"
        UPDATE Orders
        SET ConfirmClient = 1, Status = 'booking confirmed'
        WHERE OrderId = @OrderId";

            using var conn = context.CreateConnection();
            return await conn.ExecuteAsync(sql, new { OrderId = orderId }) > 0;
        }


        public async Task UpdateStatus(Guid orderId, string status)
        {
            var sql = @"UPDATE Orders SET Status = @Status WHERE OrderId = @OrderId";
            using var conn = context.CreateConnection();
            await conn.ExecuteAsync(sql, new { OrderId = orderId, Status = status });
        }



    }
}
