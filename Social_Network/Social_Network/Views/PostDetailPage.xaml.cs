using Social_Network.ViewModels;

namespace Social_Network.Views;

public partial class PostDetailPage : ContentPage
{
    public PostDetailPage(PostDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnBack(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");
}
