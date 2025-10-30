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
            using var connection = _context.CreateConnection();

            const string sql = "SELECT * FROM vendor";

            try
            {
                var result = await connection.QueryAsync<VendorModel>(sql);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching vendor data: {ex.Message}", ex);
            }
        }
    }
}
