using Social_Network.Core.Models;

namespace Social_Network.API.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(string login, string email, string password);
        Task<User?> LoginAsync(string email, string password);
    }
}
