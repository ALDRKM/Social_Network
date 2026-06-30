namespace Social_Network.Views
{
    public partial class CustomTabBar : ContentView
    {
        // Все фреймы для подсветки активной вкладки
        private Frame[] _tabs => new[] { HomeBg, SearchBg, CreateBg, ChatBg, ProfileBg };

        public CustomTabBar()
        {
            InitializeComponent();
        }

        private void SetActive(Frame active)
        {
            foreach (var tab in _tabs)
                tab.BackgroundColor = Colors.Transparent;

            active.BackgroundColor = Color.FromArgb("#C8955A");
        }

        private async void OnHomeTapped(object sender, EventArgs e)
        {
            SetActive(HomeBg);
            await Shell.Current.GoToAsync("//FeedPage");
        }

        private async void OnSearchTapped(object sender, EventArgs e)
        {
            SetActive(SearchBg);
            await Shell.Current.GoToAsync("//SearchPage");
        }

        private async void OnCreateTapped(object sender, EventArgs e)
        {
            // Создание поста — не меняем активную вкладку
            await Shell.Current.GoToAsync("CreatePostPage");
        }

        private async void OnChatTapped(object sender, EventArgs e)
        {
            SetActive(ChatBg);
            await Shell.Current.GoToAsync("//ChatListPage");
        }

        private async void OnProfileTapped(object sender, EventArgs e)
        {
            SetActive(ProfileBg);
            await Shell.Current.GoToAsync("//ProfilePage");
        }
    }
}