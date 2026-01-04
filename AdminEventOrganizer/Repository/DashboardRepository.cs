using AdminEventOrganizer.Interface;
using Models;
using Dapper;
using AdminEventOrganizer.DBContext;


namespace AdminEventOrganizer.Repository
{
    public class DashboardRepository : IDashboard
    {
        private readonly DapperDbContext _context;

        public DashboardRepository(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardSummary()
        {
            using var conn = _context.CreateConnection();

            var sql = @"
        SELECT
            -- Order Statistics
            (SELECT COUNT(*) FROM Orders) AS TotalOrders,

            (SELECT COUNT(*) FROM Orders
             WHERE Status = 'waiting validation') AS WaitingValidation,

            (SELECT COUNT(*) FROM Orders
             WHERE Status = 'vendor confirmation') AS VendorConfirmation,

            (SELECT COUNT(*) FROM Orders
             WHERE Status = 'booking confirmed') AS BookingConfirmed,

            (SELECT COUNT(*) FROM Orders
             WHERE EventDate BETWEEN GETDATE() AND DATEADD(DAY, 7, GETDATE()))
             AS UpcomingEvents,

            -- Revenue
            (SELECT ISNULL(SUM(ActualPrice),0)
             FROM VendorConfirmation
             WHERE VendorStatus = 'confirmed') AS TotalRevenue,

            -- Vendor Statistics
            (SELECT COUNT(*) FROM Vendor) AS TotalVendors,

            (SELECT COUNT(*) FROM Vendor
             WHERE Status = 'available') AS ActiveVendors,

            (SELECT COUNT(*) FROM VendorConfirmation
             WHERE VendorStatus = 'pending') AS VendorPendingConfirmation,

            (SELECT COUNT(*) FROM VendorConfirmation
             WHERE VendorStatus = 'confirmed') AS VendorAccepted,

            (SELECT COUNT(*) FROM VendorConfirmation
             WHERE VendorStatus = 'rejected') AS VendorRejected,

            -- Package Statistics
            (SELECT COUNT(*) FROM eventPackage) AS TotalPackages,

            (SELECT COUNT(*) FROM eventPackage
             WHERE Status = 'available') AS ActivePackages,

            (SELECT COUNT(*) FROM eventPackage
             WHERE Status = 'unavailable') AS InactivePackages,

            -- Category Statistics
            (SELECT COUNT(*) FROM Category) AS TotalCategories,

            -- User Statistics
            (SELECT COUNT(*) FROM Users) AS TotalUsers,

            (SELECT COUNT(*) FROM Users
             WHERE IsActive = 1) AS ActiveUsers,

            (SELECT COUNT(*) FROM Users
             WHERE Role = 'Admin') AS AdminUsers,

            (SELECT COUNT(*) FROM Users
             WHERE Role = 'Vendor') AS VendorUsers,

            (SELECT COUNT(*) FROM Users
             WHERE Role = 'Customer') AS CustomerUsers
        ";

            var dashboard = await conn.QuerySingleAsync<DashboardViewModel>(sql);

            // Get Top 3 Categories
            var categorySql = @"
                SELECT TOP 3
                    c.CategoryName,
                    COUNT(*) AS UsageCount
                FROM PackageCategory pc
                INNER JOIN Category c ON pc.CategoryId = c.CategoryId
                GROUP BY c.CategoryName
                ORDER BY COUNT(*) DESC
            ";

            dashboard.TopCategories = (await conn.QueryAsync<CategoryStat>(categorySql)).ToList();

            return dashboard;
        }
    }
}
