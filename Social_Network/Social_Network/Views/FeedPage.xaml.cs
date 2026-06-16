namespace Social_Network.Views;

public partial class FeedPage : ContentPage
{
	public FeedPage()
	{
		InitializeComponent();
	}
    private async void OnSearchTapped(object sender, EventArgs e)
    => await Shell.Current.GoToAsync("//SearchPage");

    private async void OnCreatePostTapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("CreatePostPage");

    private async void OnMessagesTapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//ChatsPage");

    private async void OnProfileTapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//ProfilePage");
}