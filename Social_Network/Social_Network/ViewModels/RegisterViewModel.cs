using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Social_Network.Constants;
using Social_Network.Helpers;
using Social_Network.Service;

namespace Social_Network.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _auth;

        public RegisterViewModel(IAuthService auth)
        {
            _auth = auth;
            Title = "Регистрация";
        }

        [ObservableProperty]
        private string login = string.Empty;
        [ObservableProperty]
        private string email = string.Empty;
        [ObservableProperty]
        private string password = string.Empty;
        [ObservableProperty]
        private string confirmPassword = string.Empty;
        [ObservableProperty]
        private string? errorMessage;

        [RelayCommand]
        private async Task Register()
        {
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(ConfirmPassword) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Заполните все поля";
                return;
            }
            if (!ValidationHelper.IsValidLogin(Login))
            {
                ErrorMessage = "Логин: 3–30 символов (буквы, цифры, . _)";
                return;
            }
            if (!ValidationHelper.IsValidEmail(Email))
            {
                ErrorMessage = "Введите корректный адрес эл. почты";
                return;
            }
            if (!ValidationHelper.IsValidPassword(Password))
            {
                ErrorMessage = "Пароль должен быть не короче 6 символов и содержать хотя бы одну букву и одну цифру";
                return;
            }
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Пароли не совпадают";
                return;
            }

            IsBusy = true;
            var (success, error) = await _auth.RegisterAsync(Login, Email, Password);
            if (!success)
            {
                IsBusy = false;
                ErrorMessage = error;
                return;
            }

            var (loginOk, loginError, userId, userLogin, avatarUrl) = await _auth.LoginAsync(Email, Password);
            IsBusy = false;

            if (!loginOk)
            {
                ErrorMessage = loginError ?? "Не удалось выполнить вход";
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            Preferences.Default.Set(AppSettings.UserIdKey, userId);
            Preferences.Default.Set(AppSettings.UserLoginKey, userLogin);
            Preferences.Default.Set(AppSettings.AvatarUrlKey, avatarUrl);
            await Shell.Current.GoToAsync("//FeedPage");
        }

        [RelayCommand]
        private async Task GoToLogin()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
