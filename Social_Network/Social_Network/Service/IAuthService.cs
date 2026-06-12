using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Service
{
    public interface IAuthService
    {
        Task<(bool Success, string? Error, int UserId, string Login, string? AvatarUrl)> LoginAsync(string email, string password);
        Task<(bool Succsess, string? Error)> RegisterAsync(string login, string email, string password);
    }
}
