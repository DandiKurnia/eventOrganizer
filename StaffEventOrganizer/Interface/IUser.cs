using Models;

namespace StaffEventOrganizer.Interface
{
    public interface IUser
    {
        Task<UserModel?> Login(string email, string password);
        Task<UserModel?> GetByEmail(string email);
    }
}
