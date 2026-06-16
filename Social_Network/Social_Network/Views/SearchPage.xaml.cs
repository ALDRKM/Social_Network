using Social_Network.ViewModels;

namespace Social_Network.Views;

public partial class SearchPage : ContentPage
{
	public SearchPage(SearchViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
    private async void OnHomeTapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//FeedPage");

    private async void OnCreatePostTapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("CreatePostPage");

    private async void OnMessagesTapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//ChatsPage");

    private async void OnProfileTapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//ProfilePage");
}