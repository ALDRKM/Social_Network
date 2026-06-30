using Social_Network.Core.Models;

namespace Social_Network.API.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> UpdateUserProfileAsync(int id, string login, string? bio, string? avatarURL);
        Task<User?> UpdateAccountAsync(int id, string login, string email);
        Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword);
        Task<bool> DeleteAccountAsync(int id);
        Task<List<User>> SearchUsersAsync(string query);
    }
}
