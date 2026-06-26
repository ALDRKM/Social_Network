using System.Net.Http.Json;
using Social_Network.Constants;

namespace Social_Network.Service
{
    public class AuthService: IAuthService
    {
        private readonly HttpClient _http;
        public AuthService(HttpClient http) => _http = http;


        private record LoginRequest(string Email, string Password);
        private record LoginResponse(int Id, string Login, string Email, string? AvatarUrl);


        public async Task<(bool Success, string? Error, int UserId, string Login, string? AvatarUrl)> LoginAsync(string email, string password)
        {
            try 
            {
                var response = await _http.PostAsJsonAsync($"{ApiConfig.BaseUrl}/auth/login", new LoginRequest(email, password));

                if (!response.IsSuccessStatusCode)
                {
                    return (false, "Неверная почта или пароль", 0, string.Empty, null);
                }

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                return (true, null, result!.Id, result!.Login, result.AvatarUrl);

            } 
            catch 
            {
                return (false, "Ошибка подключения к серверу", 0, string.Empty, null);
            }
        
        }

        public record RegisterRequest(string Login, string Email, string Password);

        public async Task<(bool Succsess, string? Error)> RegisterAsync(string login, string email, string password)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{ApiConfig.BaseUrl}/auth/register", new RegisterRequest(login,email,password));

                if (!response.IsSuccessStatusCode)
                {
                    return (false, "Email уже занят");
                }

                return (true, null);

            }
            catch
            {
                return (false, "Ошибка подключения к серверу");
            }
        }

        public async Task LogoutAsync(int userId)
        {
            try { await _http.PostAsync($"{ApiConfig.BaseUrl}/auth/logout/{userId}", null); }
            catch { }
        }

    }
}
