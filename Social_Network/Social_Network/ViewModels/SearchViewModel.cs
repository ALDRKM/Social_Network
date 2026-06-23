using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Social_Network.Constants;
using Social_Network.Core.Models;
using Social_Network.Service;

namespace Social_Network.ViewModels
{
    public partial class SearchViewModel: BaseViewModel
    {
        private readonly ISearchService _search;

        public SearchViewModel(ISearchService search)
        {
            _search = search;
            Title = "Поиск";
        }

        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<Post> Posts { get; } = new();

        [ObservableProperty]
        private string searchQuery = string.Empty;

        // true — показываем пользователей (поиск по @), иначе сетку публикаций
        [ObservableProperty]
        private bool isUsersMode;

        // Подсказка о том, что сейчас ищется
        [ObservableProperty]
        private string statusText = string.Empty;

        public async Task InitializeAsync()
        {
            await LoadRecentAsync();
        }

        private async Task LoadRecentAsync()
        {
            IsUsersMode = false;
            StatusText = "Последние публикации";
            var recent = await _search.GetRecentAsync();
            Posts.Clear();
            foreach (var p in recent) Posts.Add(p);
            Users.Clear();
        }

        // Живой поиск при вводе
        partial void OnSearchQueryChanged(string value) => _ = RunSearchAsync();

        [RelayCommand]
        private Task Search() => RunSearchAsync();

        private async Task RunSearchAsync()
        {
            var q = SearchQuery?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(q))
            {
                await LoadRecentAsync();
                return;
            }

            try
            {
                if (q.StartsWith('@'))
                {
                    // Поиск пользователей по логину
                    IsUsersMode = true;
                    var login = q.TrimStart('@');
                    StatusText = $"Поиск пользователей: @{login}";
                    var users = string.IsNullOrEmpty(login)
                        ? new List<User>()
                        : await _search.SearchUsersAsync(login);
                    Users.Clear();
                    foreach (var u in users) Users.Add(u);
                    Posts.Clear();
                }
                else if (q.StartsWith('#'))
                {
                    // Поиск публикаций по тегу
                    IsUsersMode = false;
                    var tag = q.TrimStart('#');
                    StatusText = $"Поиск по тегу: #{tag}";
                    var posts = await _search.SearchByTagAsync(tag);
                    Posts.Clear();
                    foreach (var p in posts) Posts.Add(p);
                    Users.Clear();
                }
                else
                {
                    // Поиск публикаций по тексту
                    IsUsersMode = false;
                    StatusText = $"Поиск публикаций: {q}";
                    var posts = await _search.SearchPostsAsync(q);
                    Posts.Clear();
                    foreach (var p in posts) Posts.Add(p);
                    Users.Clear();
                }
            }
            catch
            {
                StatusText = "Ошибка соединения";
            }
        }

        [RelayCommand]
        private async Task GoToUserProfile(User user)
        {
            int myId = Preferences.Default.Get(AppSettings.UserIdKey, 0);
            if (user.Id == myId)
                await Shell.Current.GoToAsync("//ProfilePage");
            else
                await Shell.Current.GoToAsync($"OtherProfilePage?userId={user.Id}");
        }

        [RelayCommand]
        private async Task GoToPost(Post post)
            => await Shell.Current.GoToAsync($"PostDetailPage?postId={post.Id}");
    }
}
