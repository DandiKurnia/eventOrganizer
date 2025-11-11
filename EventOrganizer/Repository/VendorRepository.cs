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
    }
}
