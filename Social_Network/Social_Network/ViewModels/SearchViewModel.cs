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



        [ObservableProperty]
        private bool isPostsTabSelected = true;

        public bool IsUsersTabSelected => !IsPostsTabSelected;

        public string PostsTabBackground => IsPostsTabSelected ? "#C2692A" : "#EDE5D8";
        public string UsersTabBackground => IsUsersTabSelected ? "#C2692A" : "#EDE5D8";
        public string PostsTabTextColor => IsPostsTabSelected ? "White" : "#7A5C3E";
        public string UsersTabTextColor => IsUsersTabSelected ? "White" : "#7A5C3E";

        [RelayCommand]
        private void SelectPostsTab()
        {
            IsPostsTabSelected = true;
            OnPropertyChanged(nameof(IsUsersTabSelected));
            OnPropertyChanged(nameof(PostsTabBackground));
            OnPropertyChanged(nameof(UsersTabBackground));
            OnPropertyChanged(nameof(PostsTabTextColor));
            OnPropertyChanged(nameof(UsersTabTextColor));
        }

        [RelayCommand]
        private void SelectUsersTab()
        {
            IsPostsTabSelected = false;
            OnPropertyChanged(nameof(IsUsersTabSelected));
            OnPropertyChanged(nameof(PostsTabBackground));
            OnPropertyChanged(nameof(UsersTabBackground));
            OnPropertyChanged(nameof(PostsTabTextColor));
            OnPropertyChanged(nameof(UsersTabTextColor));
        }
    }
}
