using Dapper;
using Models;
using StaffEventOrganizer.DBContext;
using StaffEventOrganizer.Interface;

namespace StaffEventOrganizer.Repository
{
    public class UserRepository : IUser
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<UserModel?> GetByEmail(string email)
        {
            var sql = "SELECT * FROM Users WHERE Email = @Email";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<UserModel>(sql, new { Email = email });
        }

        public async Task<UserModel?> Login(string email, string password)
        {
            var sql = "SELECT * FROM Users WHERE Email = @Email AND IsActive = 1";
            using var connection = _context.CreateConnection();
            var user = await connection.QuerySingleOrDefaultAsync<UserModel>(sql, new { Email = email });

            if (user == null) return null;

            // Verifikasi password dengan BCrypt
            return VerifyPassword(password, user.PasswordHash) ? user : null;
        }

        private bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
