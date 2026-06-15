using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Social_Network.Service;
using Social_Network.Constants;


namespace Social_Network.ViewModels
{
    public partial class SettingsViewModel: BaseViewModel
    {
        private readonly IUserService _user;

        public SettingsViewModel(IUserService user)
        {
            _user = user;
            Title = "Настройки";
        }

        [ObservableProperty]
        private string login = string.Empty;
        [ObservableProperty]
        private string? bio;
        [ObservableProperty]
        private string? avatarUrl;
        [ObservableProperty]
        private string? errorMessage;

        [RelayCommand]
        public async Task LoadSettings()
        {
            int userId = Preferences.Default.Get(AppSettings.UserIdKey,0);
            var user = await _user.GetUserByIdAsync(userId);
            if (user == null) return;

            Login = user.Login;
            Bio = user.Bio;
            AvatarUrl = user.AvatarUrl;
        }
        [RelayCommand]
        private async Task SaveProfile()
        {
            int userId = Preferences.Default.Get(AppSettings.UserIdKey,0);
            var user = await _user.UpdateProfileAsync(userId, Login,Bio,AvatarUrl);

            if (user == null) return;

            Preferences.Default.Set(AppSettings.UserLoginKey,user.Login);
            await Shell.Current.DisplayAlertAsync("Готово","Провиль обновлен","ОК");
        }
        [RelayCommand]
        private async Task LogOut()
        {
            Preferences.Default.Remove(AppSettings.UserIdKey);
            Preferences.Default.Remove(AppSettings.UserLoginKey);
            Preferences.Default.Remove(AppSettings.AvatarUrlKey);
            await Shell.Current.GoToAsync("//LoginPage");
        }
        [RelayCommand]
        private async Task DeleteAccount()
        {
            bool confim = await Shell.Current.DisplayAlertAsync("Удаление аккаунта","Вы уверены? Это действие необратимо.","Удалить","Отмена");
            if (!confim) return;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey,0);
            await _user.DeleteAccountAsync(userId);

            Preferences.Default.Clear();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
