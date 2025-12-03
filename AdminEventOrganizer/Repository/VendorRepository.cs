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
            return await connection.QueryAsync<VendorModel>(sql);
        }

        public async Task<VendorModel?> GetById(Guid id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT * FROM vendor WHERE VendorId = @Id";
            return await connection.QuerySingleOrDefaultAsync<VendorModel>(sql, new { Id = id });
        }

        public async Task<VendorModel> Update(VendorModel model)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"UPDATE vendor 
                                 SET CompanyName = @CompanyName, 
                                     Category = @Category,
                                     Email = @Email,
                                     Phone = @Phone,
                                     Status = @Status,
                                     Address = @Address
                                 WHERE VendorId = @VendorId";
            await connection.ExecuteAsync(sql, model);
            return model;
        }

        public async Task Delete(Guid id)
        {
            using var connection = _context.CreateConnection();
            const string sql = "DELETE FROM vendor WHERE VendorId = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
