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

        public async Task<UserModel> Create(UserModel model)
        {
            model.PasswordHash = HashPassword(model.PasswordHash);
            model.UserId = Guid.NewGuid();

            var sql = @"INSERT INTO Users (UserId, Name, Email, PasswordHash, Role, IsActive, CreatedAt, PhoneNumber)
                        VALUES (@UserId, @Name, @Email, @PasswordHash, @Role, @IsActive, @CreatedAt, @PhoneNumber)";
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

        public async Task<IEnumerable<UserModel>> GetAllUsers()
        {
            var sql = "SELECT * FROM Users WHERE Role != 'Customer' ORDER BY CreatedAt ASC";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<UserModel>(sql);
        }

        public async Task<UserModel?> GetById(Guid id)
        {
            var sql = "SELECT * FROM Users WHERE UserId = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<UserModel>(sql, new { Id = id });
        }

        public async Task<UserModel> Update(UserModel model)
        {
            var sql = @"UPDATE Users 
                SET Name = @Name,
                    Role = @Role,
                    IsActive = @IsActive,
                    PhoneNumber = @PhoneNumber
                WHERE UserId = @UserId";

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, model);
            return model;
        }


        private string HashPassword(string password)
        {
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