using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Social_Network.Constants;
using Social_Network.Service;


namespace Social_Network.ViewModels
{
    public partial class LoginViewModel: BaseViewModel
    {
        private readonly IAuthService _auth;
        public LoginViewModel(IAuthService auth)
        {
            _auth = auth;
            Title = "Вход";
        }

        [ObservableProperty]
        private string email = string.Empty;
        [ObservableProperty]
        private string password = string.Empty;
        [ObservableProperty]
        private string? errorMessage;

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Заполните все поля";
                return;
            }
            if (!Helpers.ValidationHelper.IsValidEmail(Email))
            {
                ErrorMessage = "Введите корректный адрес эл. почты";
                return;
            }

            IsBusy = true;
            ErrorMessage = null;
            var (success, error, userId, login, avatarUrl) = await _auth.LoginAsync(Email, Password);
            IsBusy = false;

            if (!success)
            {
                ErrorMessage = error;
                return;
            }
            Preferences.Default.Set(AppSettings.UserIdKey, userId);
            Preferences.Default.Set(AppSettings.UserLoginKey, login);
            Preferences.Default.Set(AppSettings.AvatarUrlKey, avatarUrl);

            await Shell.Current.GoToAsync("//FeedPage");
        }

        [RelayCommand]
        private async Task GoToRegister()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}
