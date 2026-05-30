using Social_Network.Core.Models;

namespace Social_Network.API.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> UpdateUserProfileAsync(int id, string login, string? bio, string? avatarURL);
        Task DeleteAccountAsync(int id);
    }
}
