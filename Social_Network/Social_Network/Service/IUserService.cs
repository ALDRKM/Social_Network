using Social_Network.Core.Models;

namespace Social_Network.Service
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> UpdateProfileAsync(int id, string login, string? bio, string? avatarUrl);
        Task<bool> DeleteAccountAsync(int id);
    }
}
