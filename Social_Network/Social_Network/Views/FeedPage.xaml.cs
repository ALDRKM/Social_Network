using Social_Network.ViewModels;

namespace Social_Network.Views;

public partial class FeedPage : ContentPage
{
    private readonly FeedViewModel _vm;

    public FeedPage(FeedViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadFeedCommand.ExecuteAsync(null);
    }
}
