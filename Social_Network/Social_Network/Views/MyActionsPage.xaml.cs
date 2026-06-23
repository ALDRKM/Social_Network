using Social_Network.ViewModels;

namespace Social_Network.Views;

public partial class MyActionsPage : ContentPage
{
    private readonly MyActionsViewModel _vm;

    public MyActionsPage(MyActionsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}
