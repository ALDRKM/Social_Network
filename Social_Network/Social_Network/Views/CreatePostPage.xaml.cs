using Social_Network.ViewModels;

namespace Social_Network.Views;

public partial class CreatePostPage : ContentPage
{
	public CreatePostPage(CreatePostViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
    private async void OnCloseTapped(object sender, EventArgs e)
       => await Shell.Current.GoToAsync("..");

    private void OnHeaderActionTapped(object sender, EventArgs e)
    {
        var vm = BindingContext as CreatePostViewModel;
        vm?.HeaderActionCommand.Execute(null);
    }

    private async void OnHomeTapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//FeedPage");

    private async void OnSearchTapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//SearchPage");

    private async void OnMessagesTapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//ChatListPage");

    private async void OnProfileTapped(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//ProfilePage");
}