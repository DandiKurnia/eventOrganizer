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
            o.OrderDate,
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




        public async Task<OrderModel> GetById(Guid orderId)
        {
            var sql = "SELECT * FROM Orders WHERE OrderId = @OrderId";
            using var connection = context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<OrderModel>(sql, new { OrderId = orderId });
        }
    }
}
