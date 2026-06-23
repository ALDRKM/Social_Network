using Social_Network.ViewModels;

namespace Social_Network.Views;

public partial class ProfileSettingsPage : ContentPage
{
    private readonly SettingsViewModel _vm;

    public ProfileSettingsPage(SettingsViewModel vm)
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
