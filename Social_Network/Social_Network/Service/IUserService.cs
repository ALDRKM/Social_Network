using Social_Network.Core.Models;

namespace Social_Network.Service
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> UpdateProfileAsync(int id, string login, string? bio, string? avatarUrl);
        Task<bool> UpdateAccountAsync(int id, string login, string email);
        Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword);
        Task<bool> DeleteAccountAsync(int id);
        Task<List<User>> SearchUsersAsync(string query);
    }
}
