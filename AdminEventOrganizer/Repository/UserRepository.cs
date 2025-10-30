using AdminEventOrganizer.DBContext;
using Dapper;
using Models;
using AdminEventOrganizer.Interface;

namespace AdminEventOrganizer.Repository
{
    public class UserRepository : IUser
    {
        private readonly DapperDbContext _context;

        public UserRepository(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel?> GetByEmail(string email)
        {
            var sql = "SELECT * FROM Users WHERE Email = @Email";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<UserModel>(sql, new { Email = email });
        }

        public async Task<UserModel> Register(UserModel model)
        {
            // Hash password dengan BCrypt sebelum simpan
            model.PasswordHash = HashPassword(model.PasswordHash);
            model.UserId = Guid.NewGuid();

            var sql = @"INSERT INTO Users (UserId, Username, Email, PasswordHash, Role, IsActive, CreatedAt)
                        VALUES (@UserId, @Username, @Email, @PasswordHash, @Role, @IsActive, @CreatedAt)";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, model);
            return model;
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

        private string HashPassword(string password)
        {
            // Work factor 12 (default) - semakin tinggi semakin aman tapi lebih lambat
            return BCrypt.Net.BCrypt.HashPassword(password);
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