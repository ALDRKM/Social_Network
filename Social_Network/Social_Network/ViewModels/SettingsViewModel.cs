using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Social_Network.Service;
using Social_Network.Constants;
using Social_Network.Helpers;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Social_Network.ViewModels
{
    public partial class SettingsViewModel: BaseViewModel
    {
        private readonly IUserService _user;
        private readonly IUserSettingsService _settings;
        private readonly IReportService _report;
        private readonly IAuthService _auth;
        private readonly IImageUploadService _upload;

        public SettingsViewModel(IUserService user, IUserSettingsService settings, IReportService report,
            IAuthService auth, IImageUploadService upload)
        {
            _user = user;
            _settings = settings;
            _report = report;
            _auth = auth;
            _upload = upload;
            Title = "Настройки";
        }

        // Аккаунт
        [ObservableProperty] private string login = string.Empty;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string currentPassword = string.Empty;
        [ObservableProperty] private string newPassword = string.Empty;

        // Профиль
        [ObservableProperty] private string? bio;
        [ObservableProperty] private string? avatarUrl;

        // Приватность
        [ObservableProperty] private bool isPrivate;
        [ObservableProperty] private bool notificationsEnabled = true;

        // Отчёты
        public ObservableCollection<TopPostDto> TopPosts { get; } = new();
        public ObservableCollection<TopTagDto> TopTags { get; } = new();
        [ObservableProperty] private StatsDto? stats;

        // Период для отчёта по публикациям ("за весь/указанный период")
        [ObservableProperty] private bool usePeriod;
        [ObservableProperty] private DateTime fromDate = DateTime.Today.AddMonths(-1);
        [ObservableProperty] private DateTime toDate = DateTime.Today;

        [ObservableProperty] private string? statusMessage;

        // Всплывающее уведомление о результате действия
        private static async Task Notify(string text)
        {
            try { await Toast.Make(text, ToastDuration.Short).Show(); } catch { }
        }

        [RelayCommand]
        public async Task LoadSettings()
        {
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);

            var user = await _user.GetUserByIdAsync(userId);
            if (user != null)
            {
                Login = user.Login;
                Email = user.Email ?? string.Empty;
                Bio = user.Bio;
                AvatarUrl = user.AvatarUrl;
            }

            var s = await _settings.GetAsync(userId);
            if (s != null)
            {
                IsPrivate = s.IsPrivateAccount;
                NotificationsEnabled = s.NotificationsEnabled;
            }
        }

        // Навигация к отдельным страницам настроек
        [RelayCommand] private async Task OpenPrivacy() => await Shell.Current.GoToAsync("PrivacySettingsPage");
        [RelayCommand] private async Task OpenAccount() => await Shell.Current.GoToAsync("AccountSettingsPage");
        [RelayCommand] private async Task OpenProfile() => await Shell.Current.GoToAsync("ProfileSettingsPage");
        [RelayCommand] private async Task OpenReports() => await Shell.Current.GoToAsync("ReportsPage");

        // ===== Аккаунт =====
        [RelayCommand]
        private async Task SaveAccount()
        {
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            if (!Helpers.ValidationHelper.IsValidLogin(Login))
            {
                StatusMessage = "Логин: 3–30 символов (буквы, цифры, точка, _)";
                await Notify(StatusMessage);
                return;
            }
            if (!Helpers.ValidationHelper.IsValidEmail(Email))
            {
                StatusMessage = "Некорректный email";
                await Notify(StatusMessage);
                return;
            }

            bool ok = await _user.UpdateAccountAsync(userId, Login, Email);
            if (ok)
            {
                Preferences.Default.Set(AppSettings.UserLoginKey, Login);
                StatusMessage = "Аккаунт обновлён";
            }
            else StatusMessage = "Не удалось обновить (email может быть занят)";
            await Notify(StatusMessage);
        }

        [RelayCommand]
        private async Task ChangePassword()
        {
            if (!Helpers.ValidationHelper.IsValidPassword(NewPassword))
            {
                StatusMessage = "Новый пароль должен быть не короче 6 символов и содержать хотя бы одну букву и одну цифру";
                await Notify(StatusMessage);
                return;
            }
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            bool ok = await _user.ChangePasswordAsync(userId, CurrentPassword, NewPassword);
            StatusMessage = ok ? "Пароль изменён" : "Неверный текущий пароль";
            if (ok) { CurrentPassword = string.Empty; NewPassword = string.Empty; }
            await Notify(StatusMessage);
        }

        // ===== Профиль =====
        [RelayCommand]
        private async Task PickAvatar()
        {
            try
            {
                var file = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Выберите аватар",
                    FileTypes = FilePickerFileType.Images
                });
                if (file == null) return;
                using var stream = await file.OpenReadAsync();
                var url = await _upload.UploadAsync(stream, file.FileName);
                if (!string.IsNullOrEmpty(url)) AvatarUrl = url;
            }
            catch { }
        }

        [RelayCommand]
        private async Task SaveProfile()
        {
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            var user = await _user.UpdateProfileAsync(userId, Login, Bio, AvatarUrl);
            if (user == null) { StatusMessage = "Не удалось сохранить профиль"; await Notify(StatusMessage); return; }

            Preferences.Default.Set(AppSettings.AvatarUrlKey, AvatarUrl ?? string.Empty);
            StatusMessage = "Профиль обновлён";
            await Notify(StatusMessage);
        }

        // ===== Приватность =====
        [RelayCommand]
        private async Task SavePrivacy()
        {
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            bool ok = await _settings.UpdateAsync(userId, IsPrivate, NotificationsEnabled);
            StatusMessage = ok ? "Настройки приватности сохранены" : "Не удалось сохранить";
            await Notify(StatusMessage);
        }

        // ===== Отчёты =====
        [RelayCommand]
        public async Task LoadReports()
        {
            DateTime? from = UsePeriod ? FromDate : null;
            DateTime? to = UsePeriod ? ToDate.AddDays(1).AddSeconds(-1) : null;
            var posts = await _report.TopPostsByLikesAsync(from, to);
            TopPosts.Clear();
            foreach (var p in posts) TopPosts.Add(p);

            var tags = await _report.TopTagsAsync();
            TopTags.Clear();
            foreach (var t in tags) TopTags.Add(t);

            Stats = await _report.GetStatsAsync();
        }

        // Экспорт отчётов в Excel (.xlsx).
        // На ПК — диалог выбора места; на телефоне — в «Загрузки».
        [RelayCommand]
        private async Task ExportData()
        {
            await LoadReports();

            var statsSheet = new ExcelExporter.Sheet { Name = "Статистика" };
            statsSheet.ColumnWidths.AddRange(new[] { 28d, 14d });
            statsSheet.Add("Показатель", "Значение");
            if (Stats != null)
            {
                statsSheet.Add("Пользователей", Stats.Users);
                statsSheet.Add("Публикаций", Stats.Posts);
                statsSheet.Add("Подписок", Stats.Subscriptions);
                statsSheet.Add("Сообщений в чатах", Stats.Messages);
                statsSheet.Add("Комментариев", Stats.Comments);
                statsSheet.Add("Отметок «нравится»", Stats.Likes);
            }

            var period = UsePeriod ? $"{FromDate:yyyy-MM-dd} — {ToDate:yyyy-MM-dd}" : "за всё время";
            var postsSheet = new ExcelExporter.Sheet { Name = "Популярные публикации" };
            postsSheet.ColumnWidths.AddRange(new[] { 18d, 50d, 10d, 14d });
            postsSheet.Add($"Самые популярные публикации по «нравится» ({period})");
            postsSheet.Add("Автор", "Текст", "Лайки", "Дата");
            foreach (var p in TopPosts)
                postsSheet.Add(p.Author, p.Content, p.Likes, p.CreatedAt.ToString("yyyy-MM-dd"));

            var tagsSheet = new ExcelExporter.Sheet { Name = "Популярные теги" };
            tagsSheet.ColumnWidths.AddRange(new[] { 24d, 16d });
            tagsSheet.Add("Тег", "Использований");
            foreach (var t in TopTags)
                tagsSheet.Add("#" + t.Tag, t.Count);

            var bytes = ExcelExporter.Build(new[] { statsSheet, postsSheet, tagsSheet });
            var fileName = $"otchet_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

            try
            {
                using var stream = new MemoryStream(bytes);
                var result = await FileSaver.Default.SaveAsync(fileName, stream, CancellationToken.None);
                StatusMessage = result.IsSuccessful
                    ? "Отчёт сохранён в Excel"
                    : "Экспорт отменён";
                await Notify(StatusMessage);
            }
            catch (Exception ex)
            {
                StatusMessage = "Не удалось сохранить: " + ex.Message;
                await Notify("Не удалось сохранить отчёт");
            }
        }

        // ===== Общие =====
        [RelayCommand]
        private async Task LogOut()
        {
            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            await _auth.LogoutAsync(userId);

            Preferences.Default.Remove(AppSettings.UserIdKey);
            Preferences.Default.Remove(AppSettings.UserLoginKey);
            Preferences.Default.Remove(AppSettings.AvatarUrlKey);
            await Shell.Current.GoToAsync("//LoginPage");
        }

        [RelayCommand]
        private async Task DeleteAccount()
        {
            bool confirm = await Shell.Current.DisplayAlertAsync(
                "Удаление аккаунта", "Вы уверены? Это действие необратимо.", "Удалить", "Отмена");
            if (!confirm) return;

            int userId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            await _user.DeleteAccountAsync(userId);

            Preferences.Default.Clear();
            await Shell.Current.GoToAsync("//LoginPage");
        }

        [RelayCommand]
        private async Task GoBack() => await Shell.Current.GoToAsync("..");
    }
}
