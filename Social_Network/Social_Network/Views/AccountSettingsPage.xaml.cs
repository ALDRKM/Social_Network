using Social_Network.ViewModels;

namespace Social_Network.Views;

public partial class AccountSettingsPage : ContentPage
{
    private readonly SettingsViewModel _vm;

    public AccountSettingsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadSettingsCommand.ExecuteAsync(null);
    }
}
