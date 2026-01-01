using StaffEventOrganizer.Interface;
using Models;
using Dapper;
using StaffEventOrganizer.DBContext;


namespace StaffEventOrganizer.Repository
{
    public class DashboardRepository : IDashboard
    {
        private readonly DapperContext _context;

        public DashboardRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardSummary()
        {
            using var conn = _context.CreateConnection();

            var sql = @"
        SELECT
            (SELECT COUNT(*) FROM Orders) AS TotalOrders,

            (SELECT COUNT(*) FROM Orders
             WHERE Status = 'waiting_validation') AS WaitingValidation,

            (SELECT COUNT(*) FROM Orders
             WHERE Status = 'vendor_confirmation') AS VendorConfirmation,

            (SELECT COUNT(*) FROM Orders
             WHERE Status = 'booking_confirmed') AS BookingConfirmed,

            (SELECT ISNULL(SUM(ActualPrice),0)
             FROM VendorConfirmation
             WHERE VendorStatus = 'confirmed') AS TotalRevenue,

            (SELECT COUNT(*) FROM Vendor
             WHERE Status = 'available') AS ActiveVendors,

            (SELECT COUNT(*) FROM Orders
             WHERE EventDate BETWEEN GETDATE() AND DATEADD(DAY, 7, GETDATE()))
             AS UpcomingEvents
        ";

            return await conn.QuerySingleAsync<DashboardViewModel>(sql);
        }
    }
}
