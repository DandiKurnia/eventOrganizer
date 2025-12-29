using EventOrganizer.DBContext;
using EventOrganizer.Interface;
using Models;
using Dapper;

namespace EventOrganizer.Repository
{
    public class OrderRepository : IOrder
    {
        private readonly DapperDbContext context;

        public OrderRepository(DapperDbContext dbContext)
        {
            context = dbContext;
        }

        public async Task<IEnumerable<OrderModel>> Get(Guid userId)
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
            p.PackageName AS PackageName
        FROM Orders AS o
        INNER JOIN eventPackage AS p 
            ON o.PackageEventId = p.PackageEventId
        WHERE o.UserId = @UserId
        ORDER BY o.CreatedAt DESC";

            using var connection = context.CreateConnection();
            var result = await connection.QueryAsync<OrderModel>(sql, new { UserId = userId });

            foreach (var item in result)
            {
                Console.WriteLine($"OrderId: {item.OrderId}, PackageName: {item.PackageName}");
            }

            return result;
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
            u.Name AS ClientName
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
        o.ConfirmClient,

        u.Name        AS ClientName,
        u.Email       AS ClientEmail,
        u.PhoneNumber AS ClientPhoneNumber,

        p.PackageName
    FROM Orders o
    JOIN Users u ON o.UserId = u.UserId
    JOIN eventPackage p ON o.PackageEventId = p.PackageEventId
    WHERE o.OrderId = @OrderId;
    ";

            using var connection = context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<OrderModel>(
                sql,
                new { OrderId = orderId }
            );
        }



        public async Task Create(OrderModel model)
        {
            var sql = @"
        INSERT INTO Orders
        (OrderId, UserId, PackageEventId, AdditionalRequest, EventDate, Status, CreatedAt)
        VALUES
        (@OrderId, @UserId, @PackageEventId, @AdditionalRequest, @EventDate, @Status, @CreatedAt)
    ";

            using var connection = context.CreateConnection();
            await connection.ExecuteAsync(sql, model);
        }

        public async Task<bool> ConfirmOrder(Guid orderId)
        {
            var sql = @"UPDATE Orders 
                SET ConfirmClient = 1 
                WHERE OrderId = @OrderId";

            using var connection = context.CreateConnection();
            var rows = await connection.ExecuteAsync(sql, new { OrderId = orderId });

            return rows > 0;
        }

        public async Task UpdateStatus(Guid orderId, string status)
        {
            var sql = @"UPDATE Orders SET Status = @Status WHERE OrderId = @OrderId";

            using var conn = context.CreateConnection();
            await conn.ExecuteAsync(sql, new { Status = status, OrderId = orderId });
        }




    }
}
