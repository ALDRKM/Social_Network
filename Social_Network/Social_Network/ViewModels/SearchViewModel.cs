using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
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
        public ObservableCollection<Post> Posts{ get; } = new();

        [ObservableProperty]
        private string searchQuery = string.Empty;
        [ObservableProperty]
        private bool showUsers = true;
        [ObservableProperty]
        private bool isUsersTabActive = true;
        [ObservableProperty]
        private bool isPostTabActive = false;
        [ObservableProperty]
        private string? errorMessage;

        [RelayCommand]
        private void SelecUsersTab()
        {
            ShowUsers = false;
            IsUsersTabActive = false;
            IsPostTabActive = true;
        }

        [RelayCommand]
        private async Task Search()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                Users.Clear();
                Posts.Clear();
                return;
            }

            if (IsBusy) return;

            IsBusy = true;
            ErrorMessage = null;

            try
            {
                if (ShowUsers)
                {
                    var users = await _search.SearchUsersAsync(SearchQuery);
                    Users.Clear();
                    foreach (var u in users)
                        Users.Add(u);

                    if (Users.Count == 0)
                        ErrorMessage = "Пользователи не найдены";
                }
                else
                {
                    var posts = await _search.SearchPostsAsync(SearchQuery);

                    Posts.Clear();
                    foreach (var p in posts)
                        Posts.Add(p);

                    if (Posts.Count == 0)
                        ErrorMessage = "Посты не найдены";
                }
            }
            catch
            {
                ErrorMessage = "Ошибка соединения";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToUserProfile(User user)
        {
            int myId = Preferences.Default.Get(Constants.AppSettings.UserIdKey,0);

            if (user.Id == myId)
                await Shell.Current.GoToAsync("//ProfilePage");
            else
                await Shell.Current.GoToAsync($"OtherProfilePage?userId={user.Id}");
        }

        [RelayCommand]
        private async Task GoToPost(Post post)
        {
            await Shell.Current.GoToAsync($"PostDetailPage?postId={post.Id}");
        }

        partial void OnShowUsersChanged(bool value)
        {
            Users.Clear();
            Posts.Clear();
            SearchQuery = string.Empty;
            ErrorMessage = null;
        }
    }
}
