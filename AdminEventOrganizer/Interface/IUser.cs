using Models;

namespace AdminEventOrganizer.Interface
{
    public interface IUser
    {
        Task<UserModel?> Login(string email, string password);
        Task<UserModel> Register(UserModel model);
        Task<UserModel?> GetByEmail(string email);
    }
}
