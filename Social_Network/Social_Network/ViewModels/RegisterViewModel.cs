using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Social_Network.Constants;
using Social_Network.Service;
using SQLitePCL;


namespace Social_Network.ViewModels
{
    public partial class RegisterViewModel: BaseViewModel
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
            if(string.IsNullOrWhiteSpace(Login)||string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(ConfirmPassword) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Заполните все поля";
                return;
            }
            if(Password != ConfirmPassword)
            {
                ErrorMessage = "Пароли не совпадают";
                return;
            }

            IsBusy = true;
            var (success, error) = await _auth.RegisterAsync(Login,Email,Password);
            IsBusy = false;

            if (!success)
            {
                ErrorMessage = error;
                return;
            }
            await Shell.Current.DisplayAlertAsync("Готово","Аккаунт был создан успешно! Войдите в аккаунт","ОК");
            await Shell.Current.GoToAsync("//LoginPage");
        }

        [RelayCommand]
        private async Task GoToLogin()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
