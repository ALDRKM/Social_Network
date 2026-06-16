namespace Social_Network.Views.Components
{
    public partial class NavBar : ContentView
    {
        public NavBar()
        {
            InitializeComponent();
        }

        private async void OnHomeTapped(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("//FeedPage");

        private async void OnSearchTapped(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("//SearchPage");

        private async void OnCreatePostTapped(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("CreatePostPage");

        private async void OnMessagesTapped(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("//ChatListPage");

        private async void OnProfileTapped(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("//ProfilePage");
    }
}