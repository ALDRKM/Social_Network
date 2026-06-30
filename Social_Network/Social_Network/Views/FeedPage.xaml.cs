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

        var restoreIndex = Helpers.AppState.SkipFeedReload ? Helpers.AppState.FeedScrollIndex : -1;
        Helpers.AppState.SkipFeedReload = false;

        await _vm.LoadFeedCommand.ExecuteAsync(null);

        if (restoreIndex >= 0 && restoreIndex < _vm.Posts.Count)
        {
            await Task.Delay(100);
            FeedCollection.ScrollTo(restoreIndex, position: ScrollToPosition.Start, animate: false);
        }
    }
}
